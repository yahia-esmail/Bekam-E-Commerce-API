using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using Bekam.Application.Abstraction.Contracts.Security;
using Bekam.Domain.Entities.Identity;

namespace Bekam.Infrastructure.Security;
internal class JwtProvider(IOptions<JwtOptions> options) : IJwtProvider
{
    private readonly JwtOptions _jwtOptions = options.Value; 

    public (string token, int expiresIn) GenerateToken(ApplicationUser applicationUser, IEnumerable<string> roles, IEnumerable<string> permissions)
    {
        Claim[] Claims = new Claim[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, applicationUser.Id),
            new Claim(JwtRegisteredClaimNames.Email, applicationUser.Email!),
            new Claim(JwtRegisteredClaimNames.Name, $"{applicationUser.FirstName} {applicationUser.LastName}"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(nameof(roles), JsonSerializer.Serialize(roles),JsonClaimValueTypes.JsonArray),
            new Claim(nameof(permissions), JsonSerializer.Serialize(permissions),JsonClaimValueTypes.JsonArray)
        };
        var key = new SymmetricSecurityKey(System.Text.Encoding.UTF8.GetBytes(_jwtOptions.Key));

        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);


        var jwt = new JwtSecurityToken(
            issuer: _jwtOptions.Issuer,
            audience: _jwtOptions.Audience,
            claims: Claims,
            expires: DateTime.UtcNow.AddMinutes(_jwtOptions.ExpiryMinutes),
            signingCredentials: creds
        );

        var token = new JwtSecurityTokenHandler().WriteToken(jwt);

        return (token, _jwtOptions.ExpiryMinutes * 60);
    }

    public string? ValidateToken(string token, bool validateLifetime = true)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = Encoding.UTF8.GetBytes(_jwtOptions.Key);

        try
        {
            var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = false,
                ValidateAudience = false,
                ValidateLifetime = validateLifetime,
                ClockSkew = TimeSpan.Zero
            }, out _);

            return principal.Claims
                .FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier
                                  || c.Type == JwtRegisteredClaimNames.Sub)
                ?.Value;
        }
        catch (SecurityTokenException)
        {
            return null;
        }
    }
}
