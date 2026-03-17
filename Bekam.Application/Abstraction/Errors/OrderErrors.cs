using Bekam.Application.Abstraction.Results;

namespace Bekam.Application.Abstraction.Errors;
public class OrderErrors
{
    public static readonly Error InternalError =
        new("Order.InternalError", "an error has been occurred when creating the order.", ErrorType.Failure);

    public static readonly Error OrderNotFound =
        new("Order.OrderNotFound", "Order not found.", ErrorType.NotFound);

    public static readonly Error OrderNotEligible =
        new("Order.OrderNotEligible", "Order is not eligible for payment", ErrorType.Validation);
}
