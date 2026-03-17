namespace Bekam.Domain.Entities.Cart;
public class CartItem
{
    public required int ProductId { get; set; }
    public required string ProductName { get; set; }
    public  string? PictureUrl { get; set; }
    public  string? Category { get; set; }
    public  string? Brand { get; set; }
    public decimal Price { get; set; }
    public required int Quantity { get; set; } = 1;
}
