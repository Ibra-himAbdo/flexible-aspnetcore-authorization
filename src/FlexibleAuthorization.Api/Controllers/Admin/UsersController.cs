namespace FlexibleAuthorization.Api.Controllers.Admin;

[Route("api/admin/[controller]")]
public class UsersController : BaseApiController
{
    private readonly UserManager<User> _userManager;

    public UsersController(UserManager<User> userManager)
    {
        _userManager = userManager;
    }

    [HttpGet]
    [Authorize(Permissions.ViewUsers | Permissions.ManageUsers)]
    public async Task<ActionResult<IEnumerable<UserDto>>> GetUsers()
    {
        var users = await _userManager.Users
            .OrderBy(u => u.UserName)
            .ToListAsync();

        var userDtos = new List<UserDto>();

        foreach (var user in users)
        {
            var roles = await _userManager.GetRolesAsync(user);
            var dto = new UserDto(
                user.Id,
                user.UserName ?? string.Empty,
                user.Email ?? string.Empty,
                roles.ToList()
            );

            userDtos.Add(dto);
        }

        return Ok(userDtos);
    }


    // GET: api/Admin/Users/5
    [HttpGet("{id}")]
    [Authorize(Permissions.ViewUsers)]
    public async Task<ActionResult<UserDto>> GetUser(string id)
    {
        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return NotFound();

        var dto = new UserDto(user.Id, user.UserName ?? string.Empty, user.Email ?? string.Empty);

        var roles = await _userManager.GetRolesAsync(user);

        dto.Roles.AddRange(roles);

        return dto;
    }

    // PUT: api/Admin/Users/5
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    [Authorize(Permissions.ManageUsers)]
    public async Task<IActionResult> PutUser(string id, UserDto updatedUser)
    {
        if (id != updatedUser.Id)
            return BadRequest();

        var user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return NotFound();

        user.UserName = updatedUser.UserName;
        user.Email = updatedUser.Email;

        await _userManager.UpdateAsync(user);

        var currentRoles = await _userManager.GetRolesAsync(user);
        var addedRoles = updatedUser.Roles.Except(currentRoles);
        var removedRoles = currentRoles.Except(updatedUser.Roles);

        if (addedRoles.Any())
        {
            await _userManager.AddToRolesAsync(user, addedRoles);
        }

        if (removedRoles.Any())
        {
            await _userManager.RemoveFromRolesAsync(user, removedRoles);
        }

        return NoContent();
    }
}