namespace Bekam.Domain.Entities.Orders;
public enum OrderStatus
{
    Pending,
    PaymentReceived,
    PaymentFailed,
    Shipped,
    Delivered,
    Cancelled
}
