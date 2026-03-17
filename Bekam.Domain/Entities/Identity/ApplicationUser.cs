using Microsoft.AspNetCore.Identity;
using Microsoft.VisualBasic;

namespace Bekam.Domain.Entities.Identity;
public class ApplicationUser: IdentityUser
{
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public bool IsActive { get; set; } = true;
    public DateTime JoinedAt { get; set; }
    public List<RefreshToken> RefreshTokens { get; set; } = [];
}
