namespace FlexibleAuthorization.Shared.Authorization;

public class PermissionAuthorizationHandler
    : AuthorizationHandler<PermissionAuthorizationRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionAuthorizationRequirement requirement)
    {
        var permissionClaim = context.User.FindFirst(
            c => c.Type == CustomClaimTypes.Permissions);

        if (permissionClaim is null || !int.TryParse(permissionClaim.Value, out var permissionClaimValue))
            return Task.CompletedTask;

        var userPermissions = (Permissions)permissionClaimValue;

        if ((userPermissions & requirement.Permissions) == 0)
            return Task.CompletedTask;

        context.Succeed(requirement);
        return Task.CompletedTask;
    }
}