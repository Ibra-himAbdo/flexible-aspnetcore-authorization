namespace FlexibleAuthorization.Api.Data;

public class DbInitializer(
    ApplicationDbContext context,
    UserManager<User> userManager,
    RoleManager<Role> roleManager)
{
    private const string AdministratorsRole = "Administrators";
    private const string AccountsRole = "Accounts";
    private const string OperationsRole = "Operations";

    private const string DefaultPassword = "Password123!";

    public async Task RunAsync()
    {
        await context.Database.MigrateAsync();
        
        // Check if roles table is empty
        if (await roleManager.Roles.AnyAsync())
            return; // Roles already exist, no need to seed

        // Create roles
        await roleManager.CreateAsync(
            new Role
            {
                Name = AdministratorsRole,
                NormalizedName = AdministratorsRole.ToUpper(),
                Permissions = Permissions.All
            });

        await roleManager.CreateAsync(
            new Role
            {
                Name = AccountsRole,
                NormalizedName = AccountsRole.ToUpper(),
                Permissions =
                    Permissions.ViewUsers |
                    Permissions.Counter
            });

        await roleManager.CreateAsync(
            new Role
            {
                Name = OperationsRole,
                NormalizedName = OperationsRole.ToUpper(),
                Permissions =
                    Permissions.ViewUsers |
                    Permissions.Forecast
            });

        // Ensure admin role has all permissions
        var adminRole = await roleManager.FindByNameAsync(AdministratorsRole);
        adminRole!.Permissions = Permissions.All;
        await roleManager.UpdateAsync(adminRole);

        // Create default admin user
        var adminUserName = "admin@localhost";
        var adminUser = new User { UserName = adminUserName, Email = adminUserName };
        await userManager.CreateAsync(adminUser, DefaultPassword);

        adminUser = await userManager.FindByNameAsync(adminUserName);
        if (adminUser is not null)
            await userManager.AddToRoleAsync(adminUser, AdministratorsRole);

        // Create default auditor user
        var auditorUserName = "auditor@localhost";
        var auditorUser = new User { UserName = auditorUserName, Email = auditorUserName };
        await userManager.CreateAsync(auditorUser, DefaultPassword);

        await context.SaveChangesAsync();
    }
}