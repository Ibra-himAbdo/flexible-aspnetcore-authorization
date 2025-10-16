namespace FlexibleAuthorization.Api;

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
    public async Task<ActionResult<LoginResponse>> Login([FromBody] LoginModel model)
    {
        User? user = await _userManager.FindByNameAsync(model.UserName);
        if (user is not null && await _userManager.CheckPasswordAsync(user, model.Password))
            return Ok(new LoginResponse(Token: await GenerateToken(user)));
        return Unauthorized(new { Message = "Invalid username or password" });
    }


    private async Task<string> GenerateToken(User user)
    {
        IList<string> userRoleNames = await _userManager.GetRolesAsync(user);
        List<Claim> roleClaims = [];
        Permissions aggregatedPermissions = Permissions.None;
        foreach (string roleName in userRoleNames)
        {
            Role? role = await _roleManager.FindByNameAsync(roleName);
            if (role is null) continue;
            roleClaims.Add(new Claim(ClaimTypes.Role, roleName));
            aggregatedPermissions |= role.Permissions;
        }

        List<Claim> claims =
        [
            new(ClaimTypes.NameIdentifier, user.Id),
            new(ClaimTypes.Name, user.UserName!),
            new(CustomClaimTypes.Permissions, ((int)aggregatedPermissions).ToString())
        ];
        claims.AddRange(roleClaims);

        SymmetricSecurityKey key = new(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
        SigningCredentials signingCredentials = new(key, SecurityAlgorithms.HmacSha256);

        JwtSecurityToken token = new JwtSecurityToken(
            issuer: null,
            audience: null,
            claims: claims,
            expires: DateTime.Now.AddDays(1),
            signingCredentials: signingCredentials);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }
}