using Microsoft.AspNetCore.Mvc;
using Bekam.API.Extensions;
using Bekam.Application.Abstraction.Contracts.Services.Cart;
using Bekam.Application.DTOs.Cart;

namespace Bekam.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CartController(ICartService cartService) : ControllerBase
{
    private readonly ICartService _cartService = cartService;

    [HttpGet]
    public async Task<IActionResult> GetCartWithMergeAsync([FromHeader(Name = "X-Cart-Id")] string? cartId)
    {
        var result = await _cartService.GetCartWithMergeAsync(cartId!);

        return result.ToApiResponse();
    }

    [HttpPost("items")]
    public async Task<IActionResult> AddProductAsync([FromHeader(Name = "X-Cart-Id")] string? cartId, AddCartItemDto dto)
    {
        var result = await _cartService.AddProductAsync(cartId!, dto.productId);

        return result.ToApiResponse();
    }

    [HttpPut("items/{productId}")]
    public async Task<IActionResult> UpdateProductQuantityAsync([FromHeader(Name = "X-Cart-Id")] string? cartId,
        int productId, UpdateProductQuantityDto dto)
    {
        var result = await _cartService.UpdateProductQuantityAsync(cartId!, productId, dto.quantity);

        return result.ToApiResponse();
    }

    [HttpDelete("items/{productId}")]
    public async Task<IActionResult> RemoveProductAsync([FromHeader(Name = "X-Cart-Id")] string? cartId, int productId)
    {
        var result = await _cartService.RemoveProductAsync(cartId!, productId);

        return result.ToApiResponse();
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteCart([FromHeader(Name = "X-Cart-Id")] string? cartId)
    {
        var result = await _cartService.DeleteCartAsync(cartId!);

        return result.ToApiResponse();
    }


}
