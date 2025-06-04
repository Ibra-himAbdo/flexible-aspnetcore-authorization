global using AuthorizeAttribute = FlexibleAuthorization.Shared.Authorization.AuthorizeAttribute;

namespace FlexibleAuthorization.Api.Controllers;
public class TestController : BaseApiController
{
    [HttpGet("GetAllPermissions")]
    public IActionResult GetAllPermissions()
    {
        var permissions = PermissionsProvider.GetAll();
        return Ok(permissions);
    }
    
    [HttpGet("GetPolicyNameFor/{permission}")]
    public IActionResult GetPolicyNameFor(Permissions permission)
    {
        var policyName = PolicyNameHelper.GeneratePolicyNameFor(permission);
        return Ok(policyName);
    }
    
    [HttpGet("GetPermissionsFrom/{policyName}")]
    public IActionResult GetPermissionsFrom(string policyName)
    {
        if (!PolicyNameHelper.IsValidPolicyName(policyName))
        {
            return BadRequest("Invalid policy name format.");
        }

        var permissions = PolicyNameHelper.GetPermissionsFrom(policyName);
        return Ok(permissions);
    }
    
    [HttpGet("AuthTest")]
    [Authorize(Permissions.ViewUsers)]
    public IActionResult AuthTest()
    {
        return Ok($"Authorization test passed!- {User.Identity?.Name} has permission to view users.");
    }
}