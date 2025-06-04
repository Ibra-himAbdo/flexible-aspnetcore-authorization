namespace FlexibleAuthorization.Api.Authorization;

public class ApplicationUserClaimsPrincipalFactory(
    UserManager<User> userManager,
    RoleManager<Role> roleManager,
    IOptions<IdentityOptions> optionsAccessor)
    : UserClaimsPrincipalFactory<User, Role>(
        userManager,
        roleManager,
        optionsAccessor)
{
    protected override async Task<ClaimsIdentity> GenerateClaimsAsync(User user)
    {
        var identity = await base.GenerateClaimsAsync(user);

        var userRoleNames = await UserManager.GetRolesAsync(user) ?? [];

        var userRoles = await RoleManager.Roles.Where(r =>
                userRoleNames.Contains(r.Name!))
            .ToListAsync();

        var userPermissions = userRoles.Aggregate(Permissions.None, (current, role) => current | role.Permissions);

        var permissionsValue = (int)userPermissions;

        identity.AddClaim(
            new Claim(CustomClaimTypes.Permissions, permissionsValue.ToString()));

        return identity;
    }
}