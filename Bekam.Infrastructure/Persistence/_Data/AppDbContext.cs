using Microsoft.EntityFrameworkCore;
using Bekam.Domain.Entities.Orders;
using Bekam.Domain.Entities.Product;

namespace Bekam.Infrastructure.Persistence._Data;
public class AppDbContext:DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<ProductBrand> Brands { get; set; }
    public DbSet<ProductCategory> Categories { get; set; }

    public DbSet<Order> Orders { get; set; }
    public DbSet<OrderItem> OrderItems { get; set; }
    public DbSet<DeliveryMethod> DeliveryMethods { get; set; }


    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {        
        modelBuilder.ApplyConfigurationsFromAssembly(
            typeof(AssemblyInformation).Assembly,
            t => t.Namespace!.Contains("Persistence._Data.Configs")
        );
    }
}
