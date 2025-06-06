using FlexibleAuthorization.Shared;

namespace FlexibleAuthorization.Api.Controllers.Admin;

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
        var roles = await _roleManager.Roles
            .ToListAsync();

        var roleDtos = roles
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
        var role = await _roleManager.FindByIdAsync(updatedRole.Id);

        if (role == null) return NoContent();
        
        role.Permissions = updatedRole.Permissions;
        await _roleManager.UpdateAsync(role);

        return NoContent();
    }
}