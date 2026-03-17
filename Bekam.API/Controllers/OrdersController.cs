using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Bekam.API.Extensions;
using Bekam.Application.Abstraction.Contracts.Services;
using Bekam.Application.Abstraction.Contracts.Services.Cart;
using Bekam.Application.DTOs.Orders;

namespace Bekam.API.Controllers;
[Route("api/[controller]")]
[ApiController]
[Authorize]
public class OrdersController(IOrderService orderService, ILoggedInUserService loggedInUserService) : ControllerBase
{
    private readonly IOrderService _orderService = orderService;
    private readonly string _loggedUserId = loggedInUserService.UserId!;

    [HttpPost]
    public async Task<IActionResult> CreateOrder(CreateOrderDto order)
    {
        var result = await _orderService.CreateOrderAsync(_loggedUserId, order);

        return result.ToApiResponse();
    }

    [HttpGet("{orderId}")]
    public async Task<IActionResult> GetOrder([FromRoute] int orderId)
    {
        var result = await _orderService.GetOrderByIdAsync(_loggedUserId, orderId);
        return result.ToApiResponse();
    }

    [HttpGet]
    public async Task<IActionResult> GetOrdersForUser()
    {
        var result = await _orderService.GetOrdersForUserAsync(_loggedUserId);
        return result.ToApiResponse();
    }
}
