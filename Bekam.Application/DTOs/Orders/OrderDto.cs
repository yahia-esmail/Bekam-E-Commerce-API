using Bekam.Domain.Entities.Orders;

namespace Bekam.Application.DTOs.Orders;
public class OrderDto
{
    public int OrderId { get; set; }
    public required string BuyerId { get; set; }
    public DateTime OrderDate { get; set; }
    public required Address ShippingAddress { get; set; }
    public int DeliveryMethodId { get; set; }
    public required string DeliveryMethod { get; set; }
    public decimal DeliveryCost { get; set; }
    public decimal Subtotal { get; set; }
    public required string Status { get; set; } 
    public string? PaymentIntentId { get; set; }
    public required ICollection<OrderItemDto> OrderItems { get; set; } 
    public decimal Total { get; set; }
}
