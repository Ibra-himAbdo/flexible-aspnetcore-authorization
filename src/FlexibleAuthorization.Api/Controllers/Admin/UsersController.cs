namespace FlexibleAuthorization.Api;

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
        List<User> users = await _userManager.Users
            .OrderBy(u => u.UserName)
            .ToListAsync();

        List<UserDto> userDtos = new List<UserDto>();

        foreach (User user in users)
        {
            IList<string> roles = await _userManager.GetRolesAsync(user);
            UserDto dto = new UserDto(
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
        User? user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return NotFound("User not found.");

        UserDto dto = new UserDto(user.Id, user.UserName ?? string.Empty, user.Email ?? string.Empty);

        IList<string> roles = await _userManager.GetRolesAsync(user);

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
            return BadRequest("User ID mismatch.");

        User? user = await _userManager.FindByIdAsync(id);

        if (user is null)
            return NotFound("User not found.");

        user.UserName = updatedUser.UserName;
        user.Email = updatedUser.Email;

        await _userManager.UpdateAsync(user);

        IList<string> currentRoles = await _userManager.GetRolesAsync(user);
        List<string> addedRoles = updatedUser.Roles.Except(currentRoles).ToList();
        List<string> removedRoles = currentRoles.Except(updatedUser.Roles).ToList();

        if (addedRoles.Count != 0)
            await _userManager.AddToRolesAsync(user, addedRoles);

        if (removedRoles.Count != 0)
            await _userManager.RemoveFromRolesAsync(user, removedRoles);

        return NoContent();
    }
}