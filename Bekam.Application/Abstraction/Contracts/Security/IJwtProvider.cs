using Bekam.Domain.Entities.Identity;

namespace Bekam.Application.Abstraction.Contracts.Security;
public interface IJwtProvider
{
    (string token, int expiresIn) GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles, IEnumerable<string> permissions);
    string? ValidateToken(string token, bool validateLifetime = true);
}
