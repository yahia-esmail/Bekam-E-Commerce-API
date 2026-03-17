using Microsoft.EntityFrameworkCore;
using Bekam.Application.Abstraction.Contracts.Persistence;
using Bekam.Domain.Entities.Orders;
using Bekam.Domain.Entities.Product;
using Bekam.Infrastructure.Persistence._Data;
using Bekam.Infrastructure.Persistence.GenericRepository;

namespace Bekam.Infrastructure.Persistence.Repositories;
internal class OrderItemRepository
    : GenericRepository<OrderItem, int>, IOrderItemRepository
{
    private readonly AppDbContext _context;

    public OrderItemRepository(AppDbContext context)
        : base(context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<Product>> GetTrendingProductsAsync(DateTime fromDate)
    {
        return await _context.OrderItems
            .Where(o => o.Order!.CreatedOn >= fromDate)
            .GroupBy(o => o.ProductId)
            .Select(g => new
            {
                ProductId = g.Key,
                TotalSold = g.Sum(x => x.Quantity)
            })
            .OrderByDescending(x => x.TotalSold)
            .Take(10)
            .Join(_context.Products,
                  g => g.ProductId,
                  p => p.Id,
                  (g, p) => p)
            .Include(p => p.Category)
            .Include(p => p.Brand)
            .ToListAsync();
    }
}