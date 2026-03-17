using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.categories;

namespace Bekam.Application.Abstraction.Contracts.Services.Categories;
public interface ICategoriesService
{
    Task<Result<IReadOnlyList<CategoryDto>>> GetParentCategoriesAsync();
    Task<Result<IReadOnlyList<CategoryWithCountDto>>> GetParentCategoriesWithProductsCountAsync();
    Task<Result<IReadOnlyList<CategoryDto>>> GetAllCategoriesAsync();
    Task<Result<IReadOnlyList<SubCategoryWithCountDto>>> GetSubCategoriesAsync(int parentCategoryId);
    Task<Result<CategoryDto>> GetCategoryByIdAsync(int id);
    Task<Result<CategoryDto>> CreateCategoryAsync(CreateCategoryDto dto);
    Task<Result> UpdateCategoryAsync(int id, UpdateCategoryDto dto);
    Task<Result> DeleteCategoryAsync(int id);
}
