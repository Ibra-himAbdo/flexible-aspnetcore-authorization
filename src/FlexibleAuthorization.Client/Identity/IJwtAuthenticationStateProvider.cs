namespace FlexibleAuthorization.Client;

public interface IJwtAuthenticationStateProvider
{
    Task<AuthResult> LoginAsync(LoginModel credentials);
    Task LogoutAsync();
}