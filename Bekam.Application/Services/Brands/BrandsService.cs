using Bekam.Application.Abstraction.Contracts.Persistence;
using Bekam.Application.Abstraction.Contracts.Services.Brands;
using Bekam.Application.Abstraction.Errors;
using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.Brands;
using Bekam.Domain.Entities.Product;
using Mapster;
namespace Bekam.Application.Services.Brands;
public class BrandsService(IUnitOfWork unitOfWork) : IBrandsService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;

    public async Task<Result<BrandDto>> CreateAsync(CreateBrandDto dto)
    {
        var repo = _unitOfWork.GetRepository<ProductBrand, int>();

        var normalized = dto.Name.ToUpper();

        var exists = await repo.ExistsAsync(b => b.NormalizedName == normalized);
        if (exists)
            return Result.Failure<BrandDto>(BrandErrors.DuplicateName);

        var brand = new ProductBrand
        {
            Name = dto.Name,
            NormalizedName = normalized
        };

        await repo.AddAsync(brand);
        await _unitOfWork.CompleteAsync();

        return Result.Success(brand.Adapt<BrandDto>());
    }

    public async Task<Result> DeleteAsync(int id)
    {
        var repo = _unitOfWork.GetRepository<ProductBrand, int>();

        var brand = await repo.GetByIdAsync(id);
        if (brand is null)
            return Result.Failure(BrandErrors.NotFound);

        repo.Delete(brand);
        await _unitOfWork.CompleteAsync();

        return Result.Success();
    }

    public async Task<Result<IReadOnlyList<BrandDto>>> GetAllAsync()
    {
        var repo = _unitOfWork.GetRepository<ProductBrand, int>();

        var brands = await repo.GetAllAsync();

        return Result.Success(brands.Adapt<IReadOnlyList<BrandDto>>());
    }

    public async Task<Result<BrandDto>> GetByIdAsync(int id)
    {
        var repo = _unitOfWork.GetRepository<ProductBrand, int>();

        var brand = await repo.GetByIdAsync(id);
        if (brand is null)
            return Result.Failure<BrandDto>(BrandErrors.NotFound);

        return Result.Success(brand.Adapt<BrandDto>());
    }

    public async Task<Result> UpdateAsync(int id, UpdateBrandDto dto)
    {
        var repo = _unitOfWork.GetRepository<ProductBrand, int>();

        var brand = await repo.GetByIdAsync(id);
        if (brand is null)
            return Result.Failure(BrandErrors.NotFound);

        var normalized = dto.Name.ToUpper();

        var duplicate = await repo.ExistsAsync(b =>
            b.Id != id && b.NormalizedName == normalized);

        if (duplicate)
            return Result.Failure(BrandErrors.DuplicateName);

        brand.Name = dto.Name;
        brand.NormalizedName = normalized;

        await _unitOfWork.CompleteAsync();

        return Result.Success();
    }
}
