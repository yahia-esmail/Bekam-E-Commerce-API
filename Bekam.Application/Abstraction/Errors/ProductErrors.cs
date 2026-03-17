using Bekam.Application.Abstraction.Results;

namespace Bekam.Application.Abstraction.Errors;
public static class ProductErrors
{
    public static readonly Error ProductNotFound =
        new("Product.ProductNotFound", "Product not found.", ErrorType.NotFound);

    public static readonly Error InternalError =
        new("Product.InternalError", "An error has been occurred.", ErrorType.Failure);

    public static readonly Error FailedToAddProduct =
        new("Product.FailedToAddProduct", "Failed to add product", ErrorType.Failure);

    public static readonly Error InvalidImage =
        new("Product.InvalidImage", "Invalid Image", ErrorType.Validation);

    public static readonly Error CategoryNotFound =
        new("Product.CategoryNotFound", "Category Not Found", ErrorType.NotFound);

    public static readonly Error BrandNotFound =
        new("Product.BrandNotFound", "Brand Not Found", ErrorType.NotFound);

}
