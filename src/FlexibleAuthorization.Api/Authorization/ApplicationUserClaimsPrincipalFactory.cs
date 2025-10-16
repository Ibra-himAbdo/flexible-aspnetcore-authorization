namespace FlexibleAuthorization.Api;

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
        ClaimsIdentity identity = await base.GenerateClaimsAsync(user);

        IList<string> userRoleNames = await UserManager.GetRolesAsync(user) ?? [];

        List<Role> userRoles = await RoleManager.Roles.Where(r =>
                userRoleNames.Contains(r.Name!))
            .ToListAsync();

        Permissions userPermissions =
            userRoles.Aggregate(Permissions.None, (current, role) => current | role.Permissions);

        int permissionsValue = (int)userPermissions;

        identity.AddClaim(
            new Claim(CustomClaimTypes.Permissions, permissionsValue.ToString()));

        return identity;
    }
}