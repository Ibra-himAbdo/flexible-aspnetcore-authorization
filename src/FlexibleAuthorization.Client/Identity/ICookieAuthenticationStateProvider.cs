namespace FlexibleAuthorization.Client;

public interface ICookieAuthenticationStateProvider
{
    Task<AuthResult> LoginAsync(LoginModel credentials);
    Task LogoutAsync();
}