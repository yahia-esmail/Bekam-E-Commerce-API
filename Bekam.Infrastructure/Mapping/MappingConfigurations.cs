using Mapster;
using Bekam.Application.DTOs.Auth;
using Bekam.Application.DTOs.Brands;
using Bekam.Application.DTOs.Cart;
using Bekam.Application.DTOs.categories;
using Bekam.Application.DTOs.Orders;
using Bekam.Application.DTOs.Product;
using Bekam.Application.DTOs.Users;
using Bekam.Domain.Entities.Cart;
using Bekam.Domain.Entities.Identity;
using Bekam.Domain.Entities.Orders;
using Bekam.Domain.Entities.Product;

namespace Bekam.Infrastructure.Mapping;
internal class MappingConfigurations : IRegister
{
    public void Register(TypeAdapterConfig config)
    {
        config.NewConfig<Product, ProductDTO>();

        config.NewConfig<CreateProductDto, Product>()
            .Map(dest => dest.NormalizedName, src => src.Name.ToUpper());


        config.NewConfig<RegisterRequest, ApplicationUser>()
            .Map(dest => dest.UserName, src => src.Email)
            .TwoWays();

        config.NewConfig<Cart, CartDto>()
            .Map(dest => dest.CartId, src => src.Id)
            .TwoWays();

        config.NewConfig<Product, CartItem>()
            .Map(dest => dest.ProductId, src => src.Id)
            .Map(dest => dest.ProductName, src => src.Name)
            .Map(dest => dest.Category, src => src.Category.Name)
            .Map(dest => dest.Brand, src => src.Brand.Name);

        config.NewConfig<AddressDto, Domain.Entities.Identity.Address>();

        config.NewConfig<Order, OrderDto>()
            .Map(dest => dest.OrderId, src => src.Id)
            .Map(dest => dest.DeliveryCost, src => src.DeliveryMethod.Cost)
            .Map(dest => dest.DeliveryMethod, src => src.DeliveryMethod.ShortName);

        config.NewConfig<OrderItem, OrderItemDto>()
            .Map(dest => dest.OrderItemId, src => src.Id);

        config.NewConfig<ProductCategory, CategoryDto>();

        config.NewConfig<ProductCategory, CategoryWithCountDto>()
            .Map(dest => dest.ProductsCount, src => src.SubCategories
                                                            .SelectMany(sc => sc.Products)
                                                            .Count());

        config.NewConfig<ProductCategory, SubCategoryWithCountDto>()
            .Map(dest => dest.ProductsCount, src => src.Products.Count());


        // brands
        config.NewConfig<ProductBrand, BrandDto>();

        config.NewConfig<ApplicationUser, UserDto>()
            .Map(dest => dest.FullName,
                 src => $"{src.FirstName} {src.LastName}")
            .Map(dest => dest.IsLocked,
                 src => src.LockoutEnd != null && src.LockoutEnd > DateTimeOffset.UtcNow);
    }
}
