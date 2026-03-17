using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Stripe;
using Stripe.Checkout;
using Bekam.Application.Abstraction.Contracts;
using Bekam.Application.Abstraction.Contracts.Persistence;
using Bekam.Application.Abstraction.Contracts.Services.Payments;
using Bekam.Application.Abstraction.Errors;
using Bekam.Application.Abstraction.Results;
using Bekam.Domain.Entities.Orders;
using Bekam.Domain.Specifications.Orders;

namespace Bekam.Infrastructure.Services;
internal class PaymentService : IPaymentService
{
    private readonly StripeSettings _stripeSettings;
    private readonly IConfiguration _config;
    private readonly IUnitOfWork _unitOfWork;
    private readonly IHttpContextAccessor _httpContext;

    public PaymentService(IOptions<StripeSettings> stripeSettings, IConfiguration configuration, IUnitOfWork unitOfWork, IHttpContextAccessor httpContext)
    {
        _stripeSettings = stripeSettings.Value;
        _config = configuration;
        _unitOfWork = unitOfWork;
        _httpContext = httpContext;
        StripeConfiguration.ApiKey = _stripeSettings.SecretKey;
    }

    public async Task<Result<string>> CreateCheckoutSession(int orderId)
    {
        var orderRepo = _unitOfWork.GetRepository<Order, int>();
        var spec = new OrderWithDetailsSpecification(orderId);

        if (await orderRepo.GetAsync(spec) is not { } order)
            return Result.Failure<string>(OrderErrors.OrderNotFound);

        if (order.Status != OrderStatus.Pending)
            return Result.Failure<string>(OrderErrors.OrderNotEligible);

        var lineItems = order.OrderItems.Select(item => new SessionLineItemOptions
        {
            Quantity = item.Quantity,
            PriceData = new SessionLineItemPriceDataOptions
            {
                Currency = "egp",
                UnitAmount = (long)(item.UnitPrice * 100),
                ProductData = new SessionLineItemPriceDataProductDataOptions
                {
                    Name = item.ProductName
                }
            }
        }).ToList();

        if (order.DeliveryMethod?.Cost > 0)
        {
            lineItems.Add(new SessionLineItemOptions
            {
                Quantity = 1,
                PriceData = new SessionLineItemPriceDataOptions
                {
                    Currency = "egp",
                    UnitAmount = (long)(order.DeliveryMethod.Cost * 100),
                    ProductData = new SessionLineItemPriceDataProductDataOptions
                    {
                        Name = "Delivery"
                    }
                }
            });
        }

        var options = new SessionCreateOptions
        {
            Mode = "payment",
            SuccessUrl = $"{_config["AppSettings:FrontEndUrl"]}/orders/{order.Id}",
            CancelUrl = $"{_config["AppSettings:FrontEndUrl"]}/payment-preview/{order.Id}",
            LineItems = lineItems,
            /*Metadata = new Dictionary<string, string>
        {
            { "orderId", order.Id.ToString() }
        }*/
            PaymentIntentData = new SessionPaymentIntentDataOptions
            {
                Metadata = new Dictionary<string, string>
                {
            { "orderId", order.Id.ToString() }
                }
            }
        };


        var service = new SessionService();
        var session = await service.CreateAsync(options);

        return Result.Success(session.Url);
    }

    /*public async Task<bool> Webhook()
    {
        var json = await new StreamReader(_httpContext.HttpContext!.Request.Body).ReadToEndAsync();

        var stripeEvent = EventUtility.ConstructEvent(
            json,
            _httpContext.HttpContext.Request.Headers["Stripe-Signature"],
            _stripeSettings.WhSecret
        );

        if (stripeEvent.Type == "checkout.session.completed")
        {
            var session = stripeEvent.Data.Object as Session;

            if (session?.Metadata.TryGetValue("orderId", out var orderIdStr) == true)
            {
                if (int.TryParse(orderIdStr, out var orderId))
                {
                    await HandleSuccessfulPayment(orderId);
                }
            }
        }

        return true;
    }

    private async Task HandleSuccessfulPayment(int orderId)
    {
        var orderRepo = _unitOfWork.GetRepository<Order, int>();
        var order = await orderRepo.GetByIdAsync(orderId);

        if (order is null)
            return;

        order.Status = OrderStatus.PaymentReceived;

        orderRepo.Update(order);
        await _unitOfWork.CompleteAsync();
    }*/

    public async Task<bool> Webhook()
    {
        var context = _httpContext.HttpContext!;
        context.Request.EnableBuffering();

        var json = await new StreamReader(context.Request.Body).ReadToEndAsync();
        context.Request.Body.Position = 0;

        Event stripeEvent;

        try
        {
            stripeEvent = EventUtility.ConstructEvent(
                json,
                context.Request.Headers["Stripe-Signature"],
                _stripeSettings.WhSecret.Trim()
            );
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Webhook signature error: {ex.Message}");
            return false;
        }

        switch (stripeEvent.Type)
        {
            case "payment_intent.succeeded":
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;

                    if (intent?.Metadata.TryGetValue("orderId", out var orderIdStr) == true
                        && int.TryParse(orderIdStr, out var orderId))
                    {
                        await UpdateOrderStatus(orderId, OrderStatus.PaymentReceived);
                    }

                    break;
                }
                
            case "payment_intent.payment_failed":
                {
                    var intent = stripeEvent.Data.Object as PaymentIntent;

                    if (intent?.Metadata.TryGetValue("orderId", out var orderIdStr) == true
                        && int.TryParse(orderIdStr, out var orderId))
                    {
                        await UpdateOrderStatus(orderId, OrderStatus.PaymentFailed);
                    }

                    break;
                }

            case "charge.refunded":
                {
                    var charge = stripeEvent.Data.Object as Charge;

                    if (charge?.Metadata.TryGetValue("orderId", out var orderIdStr) == true
                        && int.TryParse(orderIdStr, out var orderId))
                    {
                        await UpdateOrderStatus(orderId, OrderStatus.Cancelled);
                    }

                    break;
                }
        }

        return true;
    }

    private async Task UpdateOrderStatus(int orderId, OrderStatus status)
    {
        var orderRepo = _unitOfWork.GetRepository<Order, int>();
        var order = await orderRepo.GetByIdAsync(orderId);

        if (order is null)
            return;

        if (order.Status == status)
            return;

        order.Status = status;

        orderRepo.Update(order);
        await _unitOfWork.CompleteAsync();
    }

}