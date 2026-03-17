using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Bekam.API.Attributes.HasPermission;
using Bekam.API.Extensions;
using Bekam.Application.Abstraction.Consts;
using Bekam.Application.Abstraction.Contracts.Services.Products;
using Bekam.Application.DTOs.Product;

namespace Bekam.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class ProductsController(IProductService productService) : ControllerBase
{
    private readonly IProductService _productService = productService;

    [HttpGet("")]
    [EnableRateLimiting(RateLimiters.MixedLimiter)]
    public async Task<IActionResult> GetProducts([FromQuery] ProductSpecParams Spec)
    {
        var result = await _productService.GetProductsAsync(Spec);

        return result.ToApiResponse();
    }

    [HttpGet("{id}")]
    [EnableRateLimiting(RateLimiters.MixedLimiter)]
    public async Task<IActionResult> GetProduct([FromRoute] int id)
    {
        var result = await _productService.GetProductAsync(id);

        return result.ToApiResponse();
    }

    [HttpGet("trending")]
    [EnableRateLimiting(RateLimiters.MixedLimiter)]
    public async Task<IActionResult> GetTrendingProducts([FromQuery] int days = 7)
    {
        var result = await _productService.GetTrendingProductsAsync(days);
        return result.ToApiResponse();
    }

    [HttpPost]
    [HasPermission(Permissions.AddProducts)]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    public async Task<IActionResult> AddProduct([FromForm] CreateProductDto productDto)
    {
        var result = await _productService.AddNewProduct(productDto);

        if(result.IsSuccess)
            return CreatedAtAction(nameof(GetProduct), new { id = result.Value.Id }, result.Value);

        return result.ToApiResponse();
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateProducts)]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    public async Task<IActionResult> UpdateProduct([FromRoute] int id,[FromForm] UpdateProductDto dto)
    {
        var result = await _productService.UpdateProduct(id, dto);

        return result.IsSuccess ? NoContent() : result.ToApiResponse();
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.DeleteProducts)]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    public async Task<IActionResult> DeleteProduct(int id)
    {
        var result = await _productService.DeleteProduct(id);

        return result.IsSuccess ? NoContent() : result.ToApiResponse();
    }
}
