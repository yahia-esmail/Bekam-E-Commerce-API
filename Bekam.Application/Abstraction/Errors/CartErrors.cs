using Bekam.Application.Abstraction.Results;

namespace Bekam.Application.Abstraction.Errors;
public static class CartErrors
{
    public static readonly Error CartNotFound =
        new("Cart.CartNotFound", "Cart not found.", ErrorType.NotFound);

    public static readonly Error ProductNotInCart =
        new("Cart.ProductNotInCart", "This Product not found in the cart.", ErrorType.NotFound);

    public static readonly Error InternalError =
        new("Cart.InternalError", "An error has been occurred.", ErrorType.Failure);
}
