namespace Bekam.Application.Abstraction.Consts;
public class DefaultRoles
{
    public const string Admin = nameof(Admin);
    public const string AdminRoleId = "14572e95-ebc4-46d2-bcbd-e7b938f1f1fc";

    public const string Member = nameof(Member);
    public const string MemberRoleId = "2bbdf517-a169-49d8-b73f-1aee335d0ed9";

    public const string AdminRoleConcurrencyStamp = "d60b17a9-6f0c-4918-8b5c-d2b66d7cfe8b";
    public const string MemberRoleConcurrencyStamp = "2a404a4c-944c-4a63-911e-909bb635eac2";

    public const string ReadOnlyAdmin = nameof(ReadOnlyAdmin);
    public const string ReadOnlyAdminRoleId = "37feb38d-5d28-4356-820f-020a7c06700c";
}
