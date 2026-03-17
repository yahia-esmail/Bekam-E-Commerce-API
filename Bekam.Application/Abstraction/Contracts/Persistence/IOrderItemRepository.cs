using Bekam.Domain.Entities.Orders;
using Bekam.Domain.Entities.Product;

namespace Bekam.Application.Abstraction.Contracts.Persistence;
public interface IOrderItemRepository
    : IGenericRepository<OrderItem, int>
{
    Task<IReadOnlyList<Product>> GetTrendingProductsAsync(DateTime fromDate);
}