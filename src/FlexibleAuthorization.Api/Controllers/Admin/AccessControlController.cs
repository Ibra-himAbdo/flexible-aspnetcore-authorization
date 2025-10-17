namespace FlexibleAuthorization.Api;

[Route("api/Admin/[controller]")]
public class AccessControlController : BaseApiController
{
    private readonly RoleManager<Role> _roleManager;

    public AccessControlController(RoleManager<Role> roleManager)
    {
        _roleManager = roleManager;
    }

    [HttpGet]
    [Authorize(Permissions.ViewAccessControl)]
    public async Task<ActionResult<AccessControlVm>> GetConfiguration()
    {
        List<Role> roles = await _roleManager.Roles
            .ToListAsync();

        List<RoleDto> roleDtos = roles
            .Select(r => new RoleDto(r.Id, r.Name ?? string.Empty, r.Permissions))
            .OrderBy(r => r.Name)
            .ToList();

        return new AccessControlVm(roleDtos);
    }

    [HttpPut]
    [Authorize(Permissions.ConfigureAccessControl)]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    public async Task<IActionResult> UpdateConfiguration(RoleDto updatedRole)
    {
        Role? role = await _roleManager.FindByIdAsync(updatedRole.Id);

        if (role == null) return BadRequest("Role not found.");

        role.Permissions = updatedRole.Permissions;
        await _roleManager.UpdateAsync(role);

        return NoContent();
    }
}