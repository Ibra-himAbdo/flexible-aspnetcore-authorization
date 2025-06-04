namespace FlexibleAuthorization.Shared.Authorization;

public class PermissionAuthorizationRequirement(Permissions permission)
    : IAuthorizationRequirement
{
    public Permissions Permissions { get; } = permission;
}