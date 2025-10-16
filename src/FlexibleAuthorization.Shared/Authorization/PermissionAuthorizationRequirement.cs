namespace FlexibleAuthorization.Shared;

public class PermissionAuthorizationRequirement(Permissions permission)
    : IAuthorizationRequirement
{
    public Permissions Permissions { get; } = permission;
}