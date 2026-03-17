namespace Bekam.Application.DTOs.Orders;
public class DeliveryMethodDto
{
    public int DeliveryMethodId { get; set; }
    public required string ShortName { get; set; }
    public required string Description { get; set; }
    public decimal Cost { get; set; }
    public required string DeliveryTime { get; set; }
}
