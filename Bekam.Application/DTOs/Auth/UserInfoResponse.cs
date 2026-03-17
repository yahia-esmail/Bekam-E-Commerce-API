namespace Bekam.Application.DTOs.Auth;
public class UserInfoResponse
{
    public required string id { get; set; }
    public required string displayedName { get; set; }
    public required string email { get; set; }
    public IEnumerable<string>? roles { get; set; }

}
