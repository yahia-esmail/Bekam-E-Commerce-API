using Microsoft.AspNetCore.Identity;
using Bekam.Domain.Common;

namespace Bekam.Domain.Entities.Identity;
public class ApplicationRole : IdentityRole, IBaseAuditableEntity
{
    public bool IsDefault { get; set; }
    public bool IsDeleted { get; set; }

    public string CreatedBy { get ; set ; }
    public DateTime CreatedOn { get ; set ; }
    public string? LastModifiedBy { get ; set ; }
    public DateTime? LastModifiedOn { get ; set ; }
}
