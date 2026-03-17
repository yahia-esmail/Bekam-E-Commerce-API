using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bekam.Domain.Entities.Orders;
using Bekam.Infrastructure.Persistence._Data.Configs.Base;

namespace Bekam.Infrastructure.Persistence._Data.Configs.Orders;
internal class OrderItemConfigurations : BaseAuditableEntityConfigurations<OrderItem, int>
{
    public override void Configure(EntityTypeBuilder<OrderItem> builder)
    {
        base.Configure(builder);

        builder.Property(oi => oi.UnitPrice)
            .HasColumnType("decimal(9,2)");

        builder.HasOne(oi => oi.Order)
            .WithMany(o => o.OrderItems)
            .HasForeignKey(oi => oi.OrderId)
            .IsRequired()
            .OnDelete(DeleteBehavior.Cascade);
    }
}