using Bekam.Application.Abstraction.Results;

namespace Bekam.Application.Abstraction.Errors;
public class RoleErrors
{
    public static readonly Error RoleAlreadyExists =
        new("Roles.AlreadyExists", "Role already exists.", ErrorType.Conflict);

    public static readonly Error FailedToCreate =
        new("Roles.FailedToCreate", "Failed to create role.", ErrorType.Failure);

    public static readonly Error FailedToDelete =
        new("Roles.FailedToDelete", "Failed to delete role.", ErrorType.Failure);

    public static readonly Error FailedToUpdate =
        new("Roles.FailedToUpdate", "Failed to update role.", ErrorType.Failure);

    public static readonly Error RoleNotFound =
        new("Roles.RoleNotFound", "Role not found.", ErrorType.NotFound);

    // permissions errors

    public static readonly Error FailedToAddPermission =
        new("Roles.FailedToAddPermission", "Failed to add permission.", ErrorType.Failure);

    public static readonly Error PermissionNotFound =
        new("Roles.PermissionNotFound", "Permission not found.", ErrorType.NotFound);

    public static readonly Error FailedToRemovePermission =
        new("Roles.FailedToRemovePermission", "Failed to remove permission.", ErrorType.Failure);

    public static readonly Error PermissionAlreadyExists =
        new("Roles.PermissionAlreadyExists", "Permission already exists in this role.", ErrorType.Conflict);

    // user roles errors

    public static readonly Error FailedToAssignRole =
        new ("Roles.FailedToAssignRole", "Failed to assign role.", ErrorType.Failure);

    public static readonly Error FailedToRemoveUserRole =
        new ("Roles.FailedToRemoveUserRole", "Failed to remove user role.", ErrorType.Failure);

    public static readonly Error RoleAlreadyAssign =
        new("Roles.RoleAlreadyAssign", "Role already Assign to this user.", ErrorType.Conflict);

}