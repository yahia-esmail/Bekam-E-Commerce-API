using Microsoft.AspNetCore.Authorization;

namespace Bekam.API.Attributes.HasPermission;

public class HasPermissionAttribute(string permission) : AuthorizeAttribute(permission)
{
}
