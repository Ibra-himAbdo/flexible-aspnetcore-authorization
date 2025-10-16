namespace FlexibleAuthorization.Api;
public class TestController : BaseApiController
{
    [HttpGet("GetAllPermissions")]
    public IActionResult GetAllPermissions()
    {
        List<Permissions> permissions = PermissionsProvider.GetAll();
        return Ok(permissions);
    }
    
    [HttpGet("GetPolicyNameFor/{permission}")]
    public IActionResult GetPolicyNameFor(Permissions permission)
    {
        string policyName = PolicyNameHelper.GeneratePolicyNameFor(permission);
        return Ok(policyName);
    }
    
    [HttpGet("GetPermissionsFrom/{policyName}")]
    public IActionResult GetPermissionsFrom(string policyName)
    {
        if (!PolicyNameHelper.IsValidPolicyName(policyName))
        {
            return BadRequest("Invalid policy name format.");
        }

        Permissions permissions = PolicyNameHelper.GetPermissionsFrom(policyName);
        return Ok(permissions);
    }
    
    [HttpGet("AuthTest")]
    [Authorize(Permissions.ViewUsers)]
    public IActionResult AuthTest()
    {
        return Ok($"Authorization test passed!- {User.Identity?.Name} has permission to view users.");
    }
}