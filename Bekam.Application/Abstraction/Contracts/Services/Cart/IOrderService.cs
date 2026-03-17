using Bekam.Application.Abstraction.Results;
using Bekam.Application.DTOs.Orders;

namespace Bekam.Application.Abstraction.Contracts.Services.Cart;
public interface IOrderService
{
    Task<Result<OrderDto>> CreateOrderAsync(string buyerId, CreateOrderDto order);

    Task<Result<IEnumerable<OrderDto>>> GetOrdersForUserAsync(string buyerId);

    Task<Result<OrderDto>> GetOrderByIdAsync(string buyerId, int orderId);

    Task<Result<IEnumerable<DeliveryMethodDto>>> GetDeliveryMethodsAsync();
}
