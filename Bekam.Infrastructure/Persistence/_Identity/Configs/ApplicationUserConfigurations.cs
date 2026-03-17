using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bekam.Domain.Entities.Identity;

namespace Bekam.Infrastructure.Persistence._Identity.Configs;
internal class ApplicationUserConfigurations : IEntityTypeConfiguration<ApplicationUser>
{
    public void Configure(EntityTypeBuilder<ApplicationUser> builder)
    {
        builder.Property(u => u.FirstName)
               .IsRequired()
               .HasMaxLength(50);

        builder.Property(u => u.LastName)
                .IsRequired()
                .HasMaxLength(50);
       

        builder.Property(u => u.IsActive)
               .HasDefaultValue(true)
               .IsRequired();

        builder.OwnsMany(x => x.RefreshTokens)
                .ToTable("RefreshTokens")
                .WithOwner()
                .HasForeignKey("UserId");

        builder.Property(u => u.JoinedAt)
            .HasDefaultValueSql("GETDATE()")
            .IsRequired();
    }
}