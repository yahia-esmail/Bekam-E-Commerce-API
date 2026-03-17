using Mapster;
using Microsoft.EntityFrameworkCore;
using Bekam.Application.Abstraction.Contracts.Persistence;
using Bekam.Application.Abstraction.Contracts.Services.Categories;
using Bekam.Application.Abstraction.Contracts.Services.Helpers;
using Bekam.Application.Abstraction.Errors;
using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.categories;
using Bekam.Domain.Entities.Product;
using Bekam.Domain.Specifications.Categories;

namespace Bekam.Application.Services.Categories;
internal class CategoriesService(IUnitOfWork unitOfWork, IUrlBuilder urlBuilder, IImageService imageService) : ICategoriesService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IUrlBuilder _urlBuilder = urlBuilder;
    private readonly IImageService _imageService = imageService;

    public async Task<Result<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto)
    {
        var categoryRepo = _unitOfWork.GetRepository<ProductCategory, int>();

        if (dto.ParentCategoryId.HasValue)
        {
            if (!await categoryRepo.ExistsAsync(c => c.Id == dto.ParentCategoryId && c.ParentCategoryId == null))
                return Result.Failure<CategoryDto>(CategoryErrors.ParentNotFound);
        }

        var duplicateExists = await categoryRepo.ExistsAsync(c =>
            c.Name == dto.Name &&
            c.ParentCategoryId == dto.ParentCategoryId);

        if (duplicateExists)
            return Result.Failure<CategoryDto>(CategoryErrors.DuplicateCategoryName);

        // upload image
        string? imagePath = null;

        if (dto.Picture is not null)
        {
            var uploadResult = await _imageService
                .UploadImageAsync(dto.Picture, "categories");

            if (!uploadResult.IsSuccess)
                return Result.Failure<CategoryDto>(uploadResult.Error);

            imagePath = uploadResult.Value;
        }

        // create category
        var category = new ProductCategory
        {
            Name = dto.Name,
            ParentCategoryId = dto.ParentCategoryId,
            PictureUrl = imagePath
        };

        await categoryRepo.AddAsync(category);

        try
        {
            await _unitOfWork.CompleteAsync();
        }
        catch
        {
            if (imagePath is not null)
                _imageService.DeleteImage(imagePath);

            throw;
        }

        return await GetCategoryByIdAsync(category.Id);
    }

    public async Task<Result> DeleteCategoryAsync(int id)
    {
        var categoryRepo = _unitOfWork.GetRepository<ProductCategory, int>();

        if (await categoryRepo.GetByIdAsync(id) is not { } category )
            return Result.Failure(CategoryErrors.CategoryNotFound);

        // check has subcategories
        var hasChildren = await categoryRepo.ExistsAsync(c => c.ParentCategoryId == id);
        if (hasChildren)
            return Result.Failure(CategoryErrors.CategoryHasSubCategories);

        // check has products
        var hasProducts = await _unitOfWork.GetRepository<Product, int>().ExistsAsync(p => p.CategoryId == id);
        if (hasProducts)
            return Result.Failure(CategoryErrors.CategoryHasProducts);

        var imagePath = category.PictureUrl;

        categoryRepo.Delete(category);

        try
        {
            await _unitOfWork.CompleteAsync();
        }
        catch
        {
            throw;
        }

        if (!string.IsNullOrWhiteSpace(imagePath))
            _imageService.DeleteImage(imagePath);

        return Result.Success();
    }

    public async Task<Result<CategoryDto>> GetCategoryByIdAsync(int id)
    {
        var categoryRepo = _unitOfWork.GetRepository<ProductCategory, int>();

        if(await categoryRepo.GetByIdAsync(id) is not { } category)
            return Result.Failure<CategoryDto>(CategoryErrors.CategoryNotFound);

        var dto = category.Adapt<CategoryDto>();
        dto.PictureUrl = _urlBuilder.BuildPictureUrl(category.PictureUrl);

        return Result.Success(dto);
    }

    public async Task<Result<IReadOnlyList<CategoryDto>>> GetParentCategoriesAsync()
    {
        var repo = _unitOfWork.GetRepository<ProductCategory, int>();

        var categories = await repo.AsQueryable(new ParentCategoriesSpecifications())
            .ProjectToType<CategoryDto>()
            .ToListAsync();

        categories.ForEach(c =>
        {
            c.PictureUrl = _urlBuilder.BuildPictureUrl(c.PictureUrl);
        });

        return Result.Success(categories.Adapt<IReadOnlyList<CategoryDto>>());
    }

    public async Task<Result<IReadOnlyList<CategoryWithCountDto>>> GetParentCategoriesWithProductsCountAsync()
    {
        var repo = _unitOfWork.GetRepository<ProductCategory, int>();

        var categories = await repo.AsQueryable(new ParentCategoriesSpecifications())
            .ProjectToType<CategoryWithCountDto>()
            .ToListAsync();

        categories.ForEach(c =>
        {
            c.PictureUrl = _urlBuilder.BuildPictureUrl(c.PictureUrl);
        });

        return Result.Success(categories.Adapt<IReadOnlyList<CategoryWithCountDto>>());
    }

    public async Task<Result<IReadOnlyList<CategoryDto>>> GetAllCategoriesAsync()
    {
        var repo = _unitOfWork.GetRepository<ProductCategory, int>();
        var categories = await repo.GetAllAsync();

        foreach (var cat in categories)
        {
            cat.PictureUrl = _urlBuilder.BuildPictureUrl(cat.PictureUrl);
        }

        return Result.Success(categories.Adapt<IReadOnlyList<CategoryDto>>());
    }

    public async Task<Result<IReadOnlyList<SubCategoryWithCountDto>>> GetSubCategoriesAsync(int parentCategoryId)
    {
        var repo = _unitOfWork.GetRepository<ProductCategory, int>();

        var categories = await repo.AsQueryable(new SubCategoriesSpecifications(parentCategoryId))
            .ProjectToType<SubCategoryWithCountDto>()
            .ToListAsync();

        categories.ForEach(c =>
        {
            c.PictureUrl = _urlBuilder.BuildPictureUrl(c.PictureUrl);
        });

        return Result.Success(categories.Adapt<IReadOnlyList<SubCategoryWithCountDto>>());
    }

    public async Task<Result> UpdateCategoryAsync(int id, UpdateCategoryDto dto)
    {
        var categoryRepo = _unitOfWork.GetRepository<ProductCategory, int>();

        if (await categoryRepo.GetByIdAsync(id) is not { } category)
            return Result.Failure(CategoryErrors.CategoryNotFound);

        if (dto.ParentCategoryId.HasValue)
        {
            if (!await categoryRepo.ExistsAsync(c => c.Id == dto.ParentCategoryId && c.ParentCategoryId == null))
                return Result.Failure<CategoryDto>(CategoryErrors.ParentNotFound);
        }

        // handle duplicate
        var duplicateExists = await categoryRepo.ExistsAsync(c =>
            c.Id != id &&
            c.Name == dto.Name &&
            c.ParentCategoryId == dto.ParentCategoryId);

        if (duplicateExists)
            return Result.Failure(CategoryErrors.DuplicateCategoryName);

        string? newImagePath = null;
        var oldImagePath = category.PictureUrl;

        // handle new image
        if (dto.Picture is not null)
        {
            var uploadResult = await _imageService
                .UploadImageAsync(dto.Picture, "categories");

            if (!uploadResult.IsSuccess)
                return Result.Failure(uploadResult.Error);

            newImagePath = uploadResult.Value;
            category.PictureUrl = newImagePath;
        }

        // update info
        category.Name = dto.Name;
        category.ParentCategoryId = dto.ParentCategoryId;

        try
        {
            await _unitOfWork.CompleteAsync();
        }
        catch
        {
            // fail => remove new image
            if (newImagePath is not null)
                _imageService.DeleteImage(newImagePath);

            throw;
        }

        // success => remove old image
        if (newImagePath is not null && !string.IsNullOrWhiteSpace(oldImagePath))
            _imageService.DeleteImage(oldImagePath);

        return Result.Success();
    }
}
