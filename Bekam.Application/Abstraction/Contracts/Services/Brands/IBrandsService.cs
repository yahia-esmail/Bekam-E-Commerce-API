using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.Brands;

namespace Bekam.Application.Abstraction.Contracts.Services.Brands;
public interface IBrandsService
{
    Task<Result<IReadOnlyList<BrandDto>>> GetAllAsync();
    Task<Result<BrandDto>> GetByIdAsync(int id);
    Task<Result<BrandDto>> CreateAsync(CreateBrandDto dto);
    Task<Result> UpdateAsync(int id, UpdateBrandDto dto);
    Task<Result> DeleteAsync(int id);
}