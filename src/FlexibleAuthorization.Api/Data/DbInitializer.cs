namespace FlexibleAuthorization.Api;

public class DbInitializer
{
    private readonly ApplicationDbContext _context;
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly ILogger<DbInitializer> _logger;

    private const string AdministratorsRole = "Administrators";
    private const string AccountsRole = "Accounts";
    private const string OperationsRole = "Operations";
    private const string DefaultPassword = "Password123!";

    public DbInitializer(
        ApplicationDbContext context,
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        ILogger<DbInitializer> logger)
    {
        _context = Guard.Against.Null(context, nameof(context));
        _userManager = Guard.Against.Null(userManager, nameof(userManager));
        _roleManager = Guard.Against.Null(roleManager, nameof(roleManager));
        _logger = Guard.Against.Null(logger, nameof(logger));
    }

    public async Task RunAsync()
    {
        try
        {
            if (await _roleManager.Roles.AnyAsync())
            {
                _logger.LogInformation("Database already initialized. Skipping seeding.");
                return;
            }

            _logger.LogInformation("Starting database initialization...");

            await CreateRolesAsync();
            await CreateDefaultUsersAsync();

            await _context.SaveChangesAsync();

            _logger.LogInformation("Database initialization completed successfully.");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Database initialization failed.");
            throw;
        }
    }

    private async Task CreateRolesAsync()
    {
        try
        {
            _logger.LogInformation("Creating default roles...");

            List<Role> roles =
            [
                new()
                {
                    Name = AdministratorsRole,
                    NormalizedName = AdministratorsRole.ToUpper(),
                    Permissions = Permissions.All
                },
                new()
                {
                    Name = AccountsRole,
                    NormalizedName = AccountsRole.ToUpper(),
                    Permissions = Permissions.ViewUsers | Permissions.Counter
                },
                new()
                {
                    Name = OperationsRole,
                    NormalizedName = OperationsRole.ToUpper(),
                    Permissions = Permissions.ViewUsers | Permissions.Forecast
                }
            ];

            foreach (Role role in roles)
            {
                IdentityResult result = await _roleManager.CreateAsync(role);
                if (!result.Succeeded)
                {
                    string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to create role {RoleName}: {Errors}", role.Name, errors);
                }
                else
                {
                    _logger.LogInformation("Created role: {RoleName}", role.Name);
                }
            }

            Role? adminRole = await _roleManager.FindByNameAsync(AdministratorsRole);
            if (adminRole != null)
            {
                adminRole.Permissions = Permissions.All;
                await _roleManager.UpdateAsync(adminRole);
                _logger.LogInformation("Updated permissions for role {RoleName}.", AdministratorsRole);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating roles.");
            throw;
        }
    }

    private async Task CreateDefaultUsersAsync()
    {
        try
        {
            _logger.LogInformation("Creating default users...");

            await CreateUserAsync("admin@localhost", AdministratorsRole);
            await CreateUserAsync("auditor@localhost", null);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating default users.");
            throw;
        }
    }

    private async Task CreateUserAsync(string email, string? role)
    {
        try
        {
            User? existingUser = await _userManager.FindByNameAsync(email);
            if (existingUser != null)
            {
                _logger.LogInformation("User {Email} already exists.", email);
                return;
            }

            User user = new User { UserName = email, Email = email };
            IdentityResult result = await _userManager.CreateAsync(user, DefaultPassword);

            if (!result.Succeeded)
            {
                string errors = string.Join(", ", result.Errors.Select(e => e.Description));
                _logger.LogWarning("Failed to create user {Email}: {Errors}", email, errors);
                return;
            }

            if (!string.IsNullOrEmpty(role))
            {
                IdentityResult roleResult = await _userManager.AddToRoleAsync(user, role);
                if (!roleResult.Succeeded)
                {
                    string errors = string.Join(", ", roleResult.Errors.Select(e => e.Description));
                    _logger.LogWarning("Failed to assign role {Role} to {Email}: {Errors}", role, email, errors);
                }
                else
                {
                    _logger.LogInformation("Assigned role {Role} to user {Email}.", role, email);
                }
            }

            _logger.LogInformation("Created user {Email}.", email);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error occurred while creating user {Email}.", email);
            throw;
        }
    }
}