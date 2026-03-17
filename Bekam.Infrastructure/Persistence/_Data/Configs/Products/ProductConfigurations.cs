using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bekam.Domain.Entities.Product;
using Bekam.Infrastructure.Persistence._Data.Configs.Base;

namespace Bekam.Infrastructure.Persistence._Data.Configs.Products;
internal class ProductConfigurations : BaseAuditableEntityConfigurations<Product, int>
{
    public override void Configure(EntityTypeBuilder<Product> builder)
    {
        base.Configure(builder);

        builder.Property(P => P.Name)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(P => P.NormalizedName)
            .IsRequired()
            .HasMaxLength(100);

        builder.Property(P => P.Description)
            .IsRequired();

        builder.HasIndex(p => p.NormalizedName);
        builder.HasIndex(p => p.Price);
        builder.HasIndex(p => p.CreatedOn);

        builder.Property(P => P.Price)
            .HasColumnType("decimal(9, 2)");

        builder.HasOne(P => P.Brand)
            .WithMany(b => b.Products)
            .HasForeignKey(P => P.BrandId)
            .OnDelete(DeleteBehavior.SetNull);


        builder.HasOne(P => P.Category)
            .WithMany(c => c.Products)
            .HasForeignKey(P => P.CategoryId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}