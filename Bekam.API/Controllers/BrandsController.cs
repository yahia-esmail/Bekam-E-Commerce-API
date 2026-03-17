using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Stripe;
using Bekam.API.Attributes.HasPermission;
using Bekam.API.Extensions;
using Bekam.Application.Abstraction.Consts;
using Bekam.Application.Abstraction.Contracts.Services.Brands;
using Bekam.Application.DTOs.Brands;

namespace Bekam.API.Controllers;
[Route("api/[controller]")]
[ApiController]
public class BrandsController(IBrandsService brandsService) : ControllerBase
{
    private readonly IBrandsService _brandsService = brandsService;

    [HttpGet]
    [EnableRateLimiting(RateLimiters.MixedLimiter)]
    public async Task<IActionResult> GetAll()
    {
        var result = await _brandsService.GetAllAsync();

        return result.ToApiResponse();
    }

    [HttpGet("{id}")]
    [EnableRateLimiting(RateLimiters.MixedLimiter)]
    public async Task<IActionResult> GetById(int id)
    {
        var result  = await _brandsService.GetByIdAsync(id);

        return result.ToApiResponse();
    }

    [HttpPost]
    [HasPermission(Permissions.AddBrands)]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    public async Task<IActionResult> Create(CreateBrandDto dto)
    {
        var result = await _brandsService.CreateAsync(dto);
        if (result.IsFailure) return result.ToApiResponse();

        return CreatedAtAction(nameof(GetById),
            new { id = result.Value.Id },
            result.Value);
    }

    [HttpPut("{id}")]
    [HasPermission(Permissions.UpdateBrands)]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    public async Task<IActionResult> Update(int id, UpdateBrandDto dto)
    {
        var result = await _brandsService.UpdateAsync(id, dto);
        return result.IsSuccess ? NoContent() : result.ToApiResponse();
    }

    [HttpDelete("{id}")]
    [HasPermission(Permissions.DeleteBrands)]
    [EnableRateLimiting(RateLimiters.UserLimiter)]
    public async Task<IActionResult> Delete(int id)
    {
        var result = await _brandsService.DeleteAsync(id);
        return result.IsSuccess ? NoContent() : result.ToApiResponse();
    }
}
