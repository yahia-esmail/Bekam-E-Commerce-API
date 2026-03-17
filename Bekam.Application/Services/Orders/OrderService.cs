using Mapster;
using MapsterMapper;
using Microsoft.EntityFrameworkCore;
using Bekam.Application.Abstraction.Contracts.Persistence;
using Bekam.Application.Abstraction.Contracts.Services.Cart;
using Bekam.Application.Abstraction.Contracts.Services.Helpers;
using Bekam.Application.Abstraction.Errors;
using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.Orders;
using Bekam.Domain.Entities.Orders;
using Bekam.Domain.Entities.Product;
using Bekam.Domain.Specifications.Orders;

namespace Bekam.Application.Services.Orders;
internal class OrderService(ICartService cartService, IUnitOfWork unitOfWork, IUrlBuilder urlBuilder, IMapper mapper) : IOrderService
{
    private readonly ICartService _cartService = cartService;
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUrlBuilder _urlBuilder = urlBuilder;
    private readonly IMapper _mapper = mapper;

    public async Task<Result<OrderDto>> CreateOrderAsync(string buyerId, CreateOrderDto order)
    {
        // 1.Get cart From carts Repo
        var cartId = buyerId;
        var result = await _cartService.GetCartAsync(cartId);

        if (!result.IsSuccess)
            return Result.Failure<OrderDto>(result.Error);

        var cart = result.Value;

        // 2. Get Selected Items at cart From Products Repo
        var orderItems = new List<OrderItem>();

        if (cart.Items.Count > 0)
        {
            var productRepo = _unitOfWork.GetRepository<Product, int>();

            foreach (var item in cart.Items)
            {
                var product = await productRepo.GetByIdAsync(item.ProductId);

                if (product is not null)
                {
                    var orderItem = new OrderItem()
                    {
                        ProductId = product.Id,
                        ProductName = product.Name,
                        PictureUrl =  product.PictureUrl,
                        UnitPrice = product.Price,
                        Quantity = item.Quantity
                    };

                    orderItems.Add(orderItem);
                }
            }
        }

        // 3. Calculate SubTotal
        var orderSubtotal = orderItems.Sum(item => item.UnitPrice * item.Quantity);

        // 4. Mapping
        var orderAddress = order.ShippingAddress.Adapt<Address>();

        // 5. Create Order
        var deliveryMethod = await _unitOfWork.GetRepository<DeliveryMethod, int>().GetByIdAsync(order.DeliveryMethodId);

        var createdOrder = new Order()
        {
            BuyerId = buyerId,
            ShippingAddress = orderAddress,
            OrderItems = orderItems,
            Subtotal = orderSubtotal,
            Total = orderSubtotal + deliveryMethod.Cost,
            DeliveryMethodId = order.DeliveryMethodId,
            DeliveryMethod = deliveryMethod
        };

        await _unitOfWork.GetRepository<Order, int>().AddAsync(createdOrder);


        // 6. Save To Database 
        var created = await _unitOfWork.CompleteAsync() > 0;

        if (!created) 
            return Result.Failure<OrderDto>(OrderErrors.InternalError);

        return Result.Success(createdOrder.Adapt<OrderDto>());
    }

    public Task<Result<IEnumerable<DeliveryMethodDto>>> GetDeliveryMethodsAsync()
    {
        throw new NotImplementedException();
    }

    public async Task<Result<OrderDto>> GetOrderByIdAsync(string buyerId, int orderId)
    {
        var orderRepo = _unitOfWork.GetRepository<Order, int>();

        var spec = new OrderWithDetailsSpecification(orderId);

        if (await orderRepo.GetAsync(spec) is not { } order || order.BuyerId != buyerId)
            return Result.Failure<OrderDto>(OrderErrors.OrderNotFound);

        var dto = _mapper.Map<OrderDto>(order);

        foreach (var item in dto.OrderItems)
        {
            item.PictureUrl = _urlBuilder.BuildPictureUrl(item.PictureUrl);
        }

        return Result.Success(dto);
    }

    public async Task<Result<IEnumerable<OrderDto>>> GetOrdersForUserAsync(string buyerId)
    {
        var orderRepo = _unitOfWork.GetRepository<Order, int>();

        var spec = new OrdersWithDetailsForUserSpecification(buyerId);
        var orders = await orderRepo.AsQueryable(spec).ProjectToType<OrderDto>().ToListAsync();

        foreach (var item in orders)
            {
            foreach (var orderItem in item.OrderItems)
            {
                orderItem.PictureUrl = _urlBuilder.BuildPictureUrl(orderItem.PictureUrl);
            }
        }
        return Result.Success<IEnumerable<OrderDto>>(orders);
    }
}
