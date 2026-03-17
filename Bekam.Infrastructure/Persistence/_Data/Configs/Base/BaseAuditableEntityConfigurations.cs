using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Bekam.Domain.Common;

namespace Bekam.Infrastructure.Persistence._Data.Configs.Base;
internal class BaseAuditableEntityConfigurations<TEntity, TKey> : BaseEntityConfigurations<TEntity, TKey>
        where TEntity : BaseAuditableEntity<TKey> where TKey : IEquatable<TKey>
{
    public override void Configure(EntityTypeBuilder<TEntity> builder)
    {
        base.Configure(builder);

        builder.Property(E => E.CreatedBy)
            .IsRequired();

        builder.Property(E => E.CreatedOn)
            .IsRequired();
    }
}