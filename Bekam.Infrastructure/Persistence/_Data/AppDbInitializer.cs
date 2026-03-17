using System.Text.Json;
using Bekam.Application.Abstraction.Contracts.Persistence.DbInitializers;
using Bekam.Domain.Entities.Orders;
using Bekam.Domain.Entities.Product;
using Bekam.Infrastructure.Persistence._Common;

namespace Bekam.Infrastructure.Persistence._Data;
internal class AppDbInitializer(AppDbContext _dbContext) : DbInitializer(_dbContext), IAppDbInitializer
{
    public override async Task SeedAsync()
    {
        var basePath = Path.Combine(
            AppContext.BaseDirectory,
            "Persistence",
            "_Data",
            "Seeds"
        );

        // Seed Brands
        if (!_dbContext.Brands.Any())
        {
            var brandsData = await File.ReadAllTextAsync($"{basePath}/brands.json");
            var brands = JsonSerializer.Deserialize<List<ProductBrand>>(brandsData);


            if (brands?.Count > 0)
            {
                await _dbContext.Set<ProductBrand>().AddRangeAsync(brands);
                await _dbContext.SaveChangesAsync();
            }

        }

        // Seed Categories
        if (!_dbContext.Categories.Any())
        {
            var categoriesData = await File.ReadAllTextAsync($"{basePath}/categories.json");
            var categories = JsonSerializer.Deserialize<List<ProductCategory>>(categoriesData);


            if (categories?.Count > 0)
            {
                await _dbContext.Set<ProductCategory>().AddRangeAsync(categories);
                await _dbContext.SaveChangesAsync();
            }

        }

        // Seed Products
        if (!_dbContext.Products.Any())
        {
            var productsData = await File.ReadAllTextAsync($"{basePath}/products.json");
            var products = JsonSerializer.Deserialize<List<Product>>(productsData);


            if (products?.Count > 0)
            {
                await _dbContext.Set<Product>().AddRangeAsync(products);
                await _dbContext.SaveChangesAsync();
            }

        }

        // Seed Delivery Methods
        if (!_dbContext.DeliveryMethods.Any())
        {
            var deliveryMethodsData = await File.ReadAllTextAsync($"{basePath}/delivery.json");
            var deliveryMethods = JsonSerializer.Deserialize<List<DeliveryMethod>>(deliveryMethodsData);


            if (deliveryMethods?.Count > 0)
            {
                await _dbContext.Set<DeliveryMethod>().AddRangeAsync(deliveryMethods);
                await _dbContext.SaveChangesAsync();
            }

        }


        
    }
}