using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.common;
using Bekam.Application.DTOs.Product;

namespace Bekam.Application.Abstraction.Contracts.Services.Products;
public interface IProductService
{
    Task<Result<PaginatedResult<ProductDTO>>> GetProductsAsync(ProductSpecParams specParams);
    Task<Result<ProductDTO>> GetProductAsync(int id);
    Task<Result<IEnumerable<ProductDTO>>> GetTrendingProductsAsync(int days = 7);
    Task<Result<ProductDTO>> AddNewProduct(CreateProductDto createProductRequest);
    Task<Result> UpdateProduct(int id, UpdateProductDto updateProductRequest);
    Task<Result> DeleteProduct(int id);
    //Task<IEnumerable<BrandDto>> GetBrandsAsync();

    //Task<IEnumerable<CategoryDto>> GetCategoriesAsync();
}
