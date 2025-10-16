namespace FlexibleAuthorization.Shared;

public static class AuthorizationServiceExtensions
{
    public static Task<AuthorizationResult> AuthorizeAsync(
        this IAuthorizationService service,
        ClaimsPrincipal user,
        Permissions permissions)
        => service.AuthorizeAsync(
            user,
            PolicyNameHelper.GeneratePolicyNameFor(permissions));
}