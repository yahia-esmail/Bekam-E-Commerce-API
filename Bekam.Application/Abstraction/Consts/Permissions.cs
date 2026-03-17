

namespace Bekam.Application.Abstraction.Consts;
public static class Permissions
{
    public static string Type { get;  } = "permissions";

    public const string AddProducts = "Products:Add";
    public const string UpdateProducts = "Products:Update";
    public const string DeleteProducts = "Products:Delete";

    public const string AddCategories = "Categories:Add";
    public const string UpdateCategories = "Categories:Update";
    public const string DeleteCategories = "Categories:Delete";

    public const string AddBrands = "Brands:Add";
    public const string UpdateBrands = "Brands:Update";
    public const string DeleteBrands = "Brands:Delete";

    public const string GetUsers = "Users:Get";
    public const string AddUsers = "Users:Add";
    public const string UpdateUsers = "Users:Update";
    public const string DeleteUsers = "Users:Delete";

    public const string GetRoles = "Roles:Get";
    public const string AddRoles = "Roles:Add";
    public const string UpdateRoles = "Roles:Update";
    public const string DeleteRoles = "Roles:Delete";

    public const string GetPermissions = "Permissions:Get";
    public const string AddPermissions = "Permissions:Add";
    public const string UpdatePermissions = "Permissions:Update";
    public const string DeletePermissions = "Permissions:Delete";

    public static IList<string?> GetAllPermissions()
    {
        return typeof(Permissions)
            .GetFields().Select(x => x.GetValue(x) as string).ToList();
    }
}
