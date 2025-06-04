namespace FlexibleAuthorization.Shared.Authorization;

public class FlexibleAuthorizationPolicyProvider(
    IOptions<AuthorizationOptions> options)
    : DefaultAuthorizationPolicyProvider(options)
{
    private readonly AuthorizationOptions _options = options.Value;

    public override async Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        var policy = await base.GetPolicyAsync(policyName);

        if (policy is not null || !PolicyNameHelper.IsValidPolicyName(policyName))
            return policy;

        var permissions = PolicyNameHelper.GetPermissionsFrom(policyName);

        policy = new AuthorizationPolicyBuilder()
            .AddRequirements(new PermissionAuthorizationRequirement(permissions))
            .Build();

        _options.AddPolicy(policyName, policy);

        return policy;
    }
}