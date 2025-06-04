namespace FlexibleAuthorization.Shared.Authorization;

public static class IAuthorizationServiceExtensions
{
    public static Task<AuthorizationResult> AuthorizeAsync(
        this IAuthorizationService service,
        ClaimsPrincipal user,
        Permissions permissions)
        => service.AuthorizeAsync(
            user,
            PolicyNameHelper.GeneratePolicyNameFor(permissions));
}