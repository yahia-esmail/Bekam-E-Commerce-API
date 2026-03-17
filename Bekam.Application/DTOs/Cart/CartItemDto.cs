namespace Bekam.Application.DTOs.Cart;
public class CartItemDto
{
    public int ProductId { get; set; }
    public required string ProductName { get; set; }
    public string? PictureUrl { get; set; }
    public string? Category { get; set; }
    public string? Brand { get; set; }
    public decimal Price { get; set; }
    public int Quantity { get; set; }

};


