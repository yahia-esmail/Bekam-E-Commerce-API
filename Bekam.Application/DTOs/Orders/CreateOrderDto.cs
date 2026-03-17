using System.Text.Json.Serialization;

namespace Bekam.Application.DTOs.Orders;
public class CreateOrderDto
{
    public int DeliveryMethodId { get; set; }
    public required AddressDto ShippingAddress { get; set; }
}
