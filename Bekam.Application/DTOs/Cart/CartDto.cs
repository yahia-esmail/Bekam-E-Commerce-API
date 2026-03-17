namespace Bekam.Application.DTOs.Cart;
public class CartDto
{
    public string CartId { get; set; } = null!;
    public List<CartItemDto> Items { get; set; } = new List<CartItemDto>();
    public decimal TotalPrice 
    { 
        get => Items.Sum(i => i.Quantity * i.Price); 
    }
}
