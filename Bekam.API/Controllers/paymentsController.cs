using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Bekam.API.Extensions;
using Bekam.Application.Abstraction.Contracts.Persistence;
using Bekam.Application.Abstraction.Contracts.Services.Payments;
using Bekam.Domain.Entities.Orders;

namespace Bekam.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class paymentsController(IPaymentService paymentService) : ControllerBase
{
    private readonly IPaymentService _paymentService = paymentService;

    [Authorize]
    [HttpPost("{orderId}/checkout-session")]
    public async Task<IActionResult> CreateCheckoutSession(int orderId)
    {
        var result = await _paymentService.CreateCheckoutSession(orderId);

        return result.ToApiResponse();
    }

    [HttpPost("webhook")]
    public async Task<IActionResult> StripeWebhook()
    {
        await _paymentService.Webhook();

        return Ok();
    }

    
}
