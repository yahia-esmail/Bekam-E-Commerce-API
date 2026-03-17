using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Bekam.API.Attributes.HasPermission;
using Bekam.API.Extensions;
using Bekam.Application.Abstraction.Consts;
using Bekam.Application.Abstraction.Contracts.Services.Categories;
using Bekam.Application.Abstraction.Contracts.Services.Products;
using Bekam.Application.DTOs.categories;

namespace Bekam.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class CategoriesController(ICategoriesService categoriesService, IProductService productService) : ControllerBase
{
    private readonly ICategoriesService _categoriesService = categoriesService;
    private readonly IProductService _productService = productService;

    [HttpGet("parents")]
    [EnableRateLimiting(RateLimiters.MixedLimiter)]
    public async Task<IActionResult> GetParentCategories()
    {
        var result = await _categoriesService.GetParentCategoriesAsync();
        return result.ToApiResponse();
    }

    [HttpGet("parents/with-count")]
    [EnableRateLimiting(RateLimiters.MixedLimiter)]
    public async Task<IActionResult> GetParentCategoriesWithCount()
    {
        var result = await _categoriesService.GetParentCategoriesWithProductsCountAsync();
        return result.ToApiResponse();
    }

    [HttpGet]
    [EnableRateLimiting(RateLimiters.MixedLimiter)]
    public async Task<IActionResult> GetAllCategories()
    {
        var result = await _categoriesService.GetAllCategoriesAsync();
        return result.ToApiResponse();
    }

    [HttpGet("{parentId}/subcategories")]
    [EnableRateLimiting(RateLimiters.MixedLimiter)]
    public async Task<IActionResult> GetSubCategories(int parentId)
    {
        var result = await _categoriesService.GetSubCategoriesAsync(parentId);
        return result.ToApiResponse();
    }

    [HttpGet("{id}")]
    [EnableRateLimiting(RateLimiters.MixedLimiter)]
    public async Task<IActionResult> GetCategoryById(int id)
    {
        var result = await _categoriesService.GetCategoryByIdAsync(id);
        return result.ToApiResponse();
    }

    [HttpPost]
    [HasPermission(Permissions.AddCategories)]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    public async Task<IActionResult> CreateCategory([FromForm] CreateCategoryDto dto)
    {
        var result = await _categoriesService.CreateCategoryAsync(dto);

        if (result.IsFailure)
            return result.ToApiResponse();

        return CreatedAtAction(nameof(GetCategoryById), new { id = result.Value.Id }, result.Value);
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateCategories)]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    public async Task<IActionResult> UpdateCategory(int id, [FromForm] UpdateCategoryDto dto)
    {
        var result = await _categoriesService.UpdateCategoryAsync(id, dto);

        return result.IsSuccess ? NoContent() : result.ToApiResponse();
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.DeleteCategories)]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    public async Task<IActionResult> DeleteCategory(int id)
    {
        var result = await _categoriesService.DeleteCategoryAsync(id);

        return result.IsSuccess ? NoContent() : result.ToApiResponse();
    }
}
