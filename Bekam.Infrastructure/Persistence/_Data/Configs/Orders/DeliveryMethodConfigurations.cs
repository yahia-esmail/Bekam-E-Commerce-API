using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bekam.Domain.Entities.Orders;
using Bekam.Infrastructure.Persistence._Data.Configs.Base;

namespace LBekam.Infrastructure.Persistence._Data.Configs.Orders;

internal class DeliveryMethodConfigurations : BaseEntityConfigurations<DeliveryMethod, int>
{
    public override void Configure(EntityTypeBuilder<DeliveryMethod> builder)
    {
        base.Configure(builder);

        builder.Property(deliveryMethod => deliveryMethod.Cost)
            .HasColumnType("decimal(9, 2)");
    }
}

