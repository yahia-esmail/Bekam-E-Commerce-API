using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Bekam.Domain.Entities.Product;
using Bekam.Infrastructure.Persistence._Data.Configs.Base;

namespace Bekam.Infrastructure.Persistence._Data.Configs.Products;
internal class BrandConfigurations : BaseAuditableEntityConfigurations<ProductBrand, int>
{
    public override void Configure(EntityTypeBuilder<ProductBrand> builder)
    {
        base.Configure(builder);

        builder.Property(b => b.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(b => b.NormalizedName)
            .IsRequired()
            .HasMaxLength(100);

        builder.HasIndex(b => b.NormalizedName)
            .IsUnique();

    }
}
