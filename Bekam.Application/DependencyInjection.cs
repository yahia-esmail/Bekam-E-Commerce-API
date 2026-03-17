using FluentValidation;
using Microsoft.Extensions.DependencyInjection;
using Bekam.Application.Abstraction.Contracts.Services.Brands;
using Bekam.Application.Abstraction.Contracts.Services.Cart;
using Bekam.Application.Abstraction.Contracts.Services.Categories;
using Bekam.Application.Abstraction.Contracts.Services.Products;
using Bekam.Application.Services.Brands;
using Bekam.Application.Services.Cart;
using Bekam.Application.Services.Categories;
using Bekam.Application.Services.Products;

namespace Bekam.Application;

public static class DependencyInjection
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        services.AddValidatorsFromAssembly(typeof(AssemblyInformation).Assembly);

        services
            .AddScoped<IProductService, ProductService>()
            .AddScoped<ICartService, CartService>()
            .AddScoped<IOrderService, Services.Orders.OrderService>()
            .AddScoped<ICategoriesService, CategoriesService>()
            .AddScoped<IBrandsService, BrandsService>();

        return services; 
    }
}
