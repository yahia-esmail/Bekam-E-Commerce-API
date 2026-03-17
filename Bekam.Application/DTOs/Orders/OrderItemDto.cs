namespace Bekam.Application.DTOs.Orders;
public class OrderItemDto
{
    public int OrderItemId { get; set; }
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public string? PictureUrl { get; set; }
    public decimal UnitPrice { get; set; }
    public int Quantity { get; set; }
}
