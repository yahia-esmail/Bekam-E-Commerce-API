namespace Bekam.Application.DTOs.Users;
public class UserDto
{
    public required string Id { get; set; }
    public required string UserName { get; set; }
    public required string FullName { get; set; }
    public required string Email { get; set; }
    public required bool EmailConfirmed { get; set; }
    public required bool IsActive { get; set; }
    public bool IsLocked { get; set; }
    public DateTime JoinedAt { get; set; }
    public IList<string> Roles { get; set; } = new List<string>();
}
