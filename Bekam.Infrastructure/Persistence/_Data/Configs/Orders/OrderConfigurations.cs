using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bekam.Domain.Entities.Orders;
using Bekam.Infrastructure.Persistence._Data.Configs.Base;

namespace Bekam.Infrastructure.Persistence._Data.Configs.Orders;
internal class OrderConfigurations : BaseAuditableEntityConfigurations<Order, int>
{
    public override void Configure(EntityTypeBuilder<Order> builder)
    {
        base.Configure(builder);

        builder.Property(o => o.BuyerId)
            .IsRequired();

        builder.Property(o => o.OrderDate)
            .IsRequired();

        builder.Property(o => o.Subtotal)
            .HasColumnType("decimal(9,2)");

        builder.Property(o => o.Total)
            .HasColumnType("decimal(9,2)");

        builder.Property(o => o.Status)
            .HasConversion<string>()
            .IsRequired();

        builder.OwnsOne(o => o.ShippingAddress, shippingAddress => shippingAddress.WithOwner());

        builder.HasOne(o => o.DeliveryMethod)
            .WithMany()
            .HasForeignKey(o => o.DeliveryMethodId)
            .OnDelete(DeleteBehavior.SetNull);        

    }
}