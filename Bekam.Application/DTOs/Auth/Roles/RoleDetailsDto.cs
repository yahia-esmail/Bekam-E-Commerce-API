namespace Bekam.Application.DTOs.Auth.Roles;
public class RoleDetailsDto
{
    public required string Id { get; set; }
    public required string Name { get; set; }
    public IReadOnlyList<string> Permissions { get; set; } = [];
}
