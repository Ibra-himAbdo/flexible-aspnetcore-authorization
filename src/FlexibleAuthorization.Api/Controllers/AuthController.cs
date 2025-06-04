namespace FlexibleAuthorization.Api.Controllers;

public class AuthController : BaseApiController
{
    private readonly UserManager<User> _userManager;
    private readonly RoleManager<Role> _roleManager;
    private readonly IConfiguration _configuration;

    public AuthController(
        UserManager<User> userManager,
        RoleManager<Role> roleManager,
        IConfiguration configuration)
    {
        _userManager = userManager;
        _roleManager = roleManager;
        _configuration = configuration;
    }

    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginModel model)
    {
        var user = await _userManager.FindByNameAsync(model.UserName);
        if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password))
            return Ok(new { Token = await GenerateToken(user) });
        return Unauthorized(new { Message = "Invalid username or password" });
    }


    private async Task<string> GenerateToken(User user)
    {
        var userRoleNames = await _userManager.GetRolesAsync(user);
        var roleClaims = new List<Claim>();
        var aggregatedPermissions = Permissions.None;
        foreach (var roleName in userRoleNames)
        {
            var role = await _roleManager.FindByNameAsync(roleName);
            if (role is null) continue;
            roleClaims.Add(new Claim(ClaimTypes.Role, roleName));
            aggregatedPermissions |= role.Permissions;
        }

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.NameIdentifier, user.Id),
            new Claim(ClaimTypes.Name, user.UserName!),
            new Claim(CustomClaimTypes.Permissions, ((int)aggregatedPermissions).ToString())
        };
        claims.AddRange(roleClaims);

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}