using Bekam.Domain.Common;

namespace Bekam.Domain.Entities.Orders;
public class OrderItem : BaseAuditableEntity<int>
{
    public int ProductId { get; set; }
    public string ProductName { get; set; } = null!;
    public string? PictureUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }

    public int OrderId { get; set; }
    public Order Order { get; set; } = null!;
}
