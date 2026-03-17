using Bekam.Domain.Common;

namespace Bekam.Domain.Entities.Orders;
public class Order :BaseAuditableEntity<int>
{
    public required string BuyerId { get; set; }
    public DateTime OrderDate { get; set; } = DateTime.Now;
    public required Address ShippingAddress { get; set; }
    public int? DeliveryMethodId { get; set; }
    public DeliveryMethod? DeliveryMethod { get; set; }
    public decimal Subtotal { get; set; }
    public decimal Total { get; set; }
    public OrderStatus Status { get; set; } = OrderStatus.Pending;
    public string PaymentIntentId { get; set; } = string.Empty;
    public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
}
