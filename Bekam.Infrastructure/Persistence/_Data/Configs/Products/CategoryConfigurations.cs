using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bekam.Domain.Entities.Product;
using Bekam.Infrastructure.Persistence._Data.Configs.Base;

namespace Bekam.Infrastructure.Persistence._Data.Configs.Products;
internal class CategoryConfigurations : BaseAuditableEntityConfigurations<ProductCategory, int>
{
    public override void Configure(EntityTypeBuilder<ProductCategory> builder)
    {
        base.Configure(builder);

        builder.Property(C => C.Name)
            .IsRequired()
            .HasMaxLength(100); ;

        builder.HasMany(C => C.SubCategories)
            .WithOne(C => C.ParentCategory)
            .HasForeignKey(C => C.ParentCategoryId)
            .OnDelete(Microsoft.EntityFrameworkCore.DeleteBehavior.Restrict);


        builder.HasIndex(C => C.ParentCategoryId);

        builder.HasIndex(c => new { c.Name, c.ParentCategoryId })
       .IsUnique();

        builder.Property(c => c.PictureUrl)
       .HasMaxLength(500);
    }
}
