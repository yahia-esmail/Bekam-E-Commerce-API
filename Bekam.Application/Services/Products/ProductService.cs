using Mapster;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Bekam.Application.Abstraction.Contracts.Persistence;
using Bekam.Application.Abstraction.Contracts.Services.Helpers;
using Bekam.Application.Abstraction.Contracts.Services.Products;
using Bekam.Application.Abstraction.Errors;
using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.common;
using Bekam.Application.DTOs.Product;
using Bekam.Domain.Entities.Product;
using Bekam.Domain.Specifications.Products;

namespace Bekam.Application.Services.Products;
internal class ProductService(IUnitOfWork unitOfWork, IConfiguration configuration, IUrlBuilder urlBuilder, IImageService imageService) : IProductService
{
    private readonly IUnitOfWork _unitOfWork = unitOfWork;
    private readonly IConfiguration _configuration = configuration;
    private readonly IUrlBuilder _urlBuilder = urlBuilder;
    private readonly IImageService _imageService = imageService;

    public async Task<Result<ProductDTO>> AddNewProduct(CreateProductDto createProductRequest)
    {
        var categoryRepo = _unitOfWork.GetRepository<ProductCategory, int>();
        if (!await categoryRepo.ExistsAsync(c => c.Id == createProductRequest.CategoryId))
            return Result.Failure<ProductDTO>(ProductErrors.CategoryNotFound);

        var brandRepo = _unitOfWork.GetRepository<ProductBrand, int>();
        if (!await brandRepo.ExistsAsync(b => b.Id == createProductRequest.BrandId))
            return Result.Failure<ProductDTO>(ProductErrors.BrandNotFound);

        var productRepo = _unitOfWork.GetRepository<Product, int>();

        var uploadImageResult = await _imageService.UploadImageAsync(createProductRequest.Picture, "products");

        if (!uploadImageResult.IsSuccess)
            return Result.Failure<ProductDTO>(uploadImageResult.Error);

        var product = createProductRequest.Adapt<Product>();
        product.PictureUrl = uploadImageResult.Value;

        await productRepo.AddAsync(product);

        try
        {
            await _unitOfWork.CompleteAsync();
        }
        catch (Exception ex)
        {
            _imageService.DeleteImage(uploadImageResult.Value);
            throw;
        }

        var productDto = await GetProductAsync(product.Id);

        return productDto.IsSuccess
        ? Result.Success(productDto.Value)
        : Result.Failure<ProductDTO>(productDto.Error);
    }

    public async Task<Result> DeleteProduct(int id)
    {
        var productRepo = _unitOfWork.GetRepository<Product, int>();

        if (await productRepo.GetByIdAsync(id) is not { } product)
            return Result.Failure(ProductErrors.ProductNotFound);

        var imagePath = product.PictureUrl;

        productRepo.Delete(product);

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

    public async Task<Result<ProductDTO>> GetProductAsync(int id)
    {
        var productsRepo = _unitOfWork.GetRepository<Product, int>();

        var spec = new ProductWithDetailsSpecification(id);
        if (await productsRepo.GetAsync(spec) is not { } product)
            return Result.Failure<ProductDTO>(ProductErrors.ProductNotFound);

        product.PictureUrl = _urlBuilder.BuildPictureUrl(product.PictureUrl);

        return Result.Success(product.Adapt<ProductDTO>());
    }


    public async Task<Result<PaginatedResult<ProductDTO>>> GetProductsAsync(ProductSpecParams specParams)
    {
        var productsRepo = _unitOfWork.GetRepository<Product, int>();
        var spec = new ProductWithBrandAndCategorySpecifications(
            specParams.Sort,
            specParams.BrandId,
            specParams.CategoryId,
            specParams.PageSize,
            specParams.PageNumber,
            specParams.Search);

        var products = await productsRepo.AsQueryable(spec).ProjectToType<ProductDTO>().ToListAsync();

        products.ForEach(products =>
        {
            products.PictureUrl = _urlBuilder.BuildPictureUrl(products.PictureUrl);
        });

        spec.DisablePaging();
        var totalCount = await productsRepo.GetCountAsync(spec);

        var result = new PaginatedResult<ProductDTO>(products, specParams.PageNumber, specParams.PageSize, totalCount);

        return Result.Success(result);
    }

    public async Task<Result<IEnumerable<ProductDTO>>> GetTrendingProductsAsync(int days = 7)
    {
        var fromDate = DateTime.UtcNow.AddDays(-days);

        var trendingProducts = await _unitOfWork.OrderItems.GetTrendingProductsAsync(fromDate);

        var dtos = trendingProducts.Select(p =>
        {
            p.PictureUrl = _urlBuilder.BuildPictureUrl(p.PictureUrl);
            return p.Adapt<ProductDTO>();
        });

        return Result.Success(dtos);

    }

    public async Task<Result> UpdateProduct(int id, UpdateProductDto request)
    {
        var productRepo = _unitOfWork.GetRepository<Product, int>();
        if (await productRepo.GetByIdAsync(id) is not { } product )
            return Result.Failure(ProductErrors.ProductNotFound);

        var brandRepo = _unitOfWork.GetRepository<ProductBrand, int>();
        if (!await brandRepo.ExistsAsync(b => b.Id == request.BrandId))
            return Result.Failure(ProductErrors.BrandNotFound);

        var categoryRepo = _unitOfWork.GetRepository<ProductCategory, int>();
        if (!await categoryRepo.ExistsAsync(c => c.Id == request.CategoryId))
            return Result.Failure(ProductErrors.CategoryNotFound);

        string? newImagePath = null;
        string? oldImagePath = product.PictureUrl;

        if (request.Picture is not null)
        {
            var uploadResult = await _imageService
                .UploadImageAsync(request.Picture, "products");

            if (!uploadResult.IsSuccess)
                return Result.Failure(uploadResult.Error);

            newImagePath = uploadResult.Value;
            product.PictureUrl = newImagePath;
        }

        product.Name = request.Name;
        product.NormalizedName = request.Name.ToUpper();
        product.Description = request.Description;
        product.Price = request.Price;
        product.BrandId = request.BrandId;
        product.CategoryId = request.CategoryId;

        try
        {
            await _unitOfWork.CompleteAsync();
        }
        catch
        {
            if (newImagePath is not null)
                _imageService.DeleteImage(newImagePath);
            throw;
        }

        if (newImagePath is not null && oldImagePath is not null)
            _imageService.DeleteImage(oldImagePath);

        return Result.Success();
    }
}
