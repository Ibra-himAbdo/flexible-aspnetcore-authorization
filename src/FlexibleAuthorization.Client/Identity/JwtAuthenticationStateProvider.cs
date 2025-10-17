namespace FlexibleAuthorization.Client;

public class JwtAuthenticationStateProvider(IAuthApi authApi, ILocalStorageService localStorage)
    : AuthenticationStateProvider, IJwtAuthenticationStateProvider
{
    public const string AuthLocalStorageKey = "auth_token";
    private readonly ClaimsPrincipal _unauthenticated = new(new ClaimsIdentity());

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsPrincipal user = _unauthenticated;

        try
        {
            using CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromSeconds(15));
            string? token = await localStorage.GetItemAsync<string>(AuthLocalStorageKey, cancellationTokenSource.Token);
            if (!string.IsNullOrWhiteSpace(token))
            {
                JwtSecurityTokenHandler handler = new();
                JwtSecurityToken? jwtToken = handler.ReadJwtToken(token);
                ClaimsIdentity claimsIdentity =
                    new(jwtToken.Claims, nameof(JwtAuthenticationStateProvider));
                user = new ClaimsPrincipal(claimsIdentity);
            }
        }
        catch
        {
            //Logging
        }


        return new AuthenticationState(user);
    }

    public async Task<AuthResult> LoginAsync(LoginModel credentials)
    {
        try
        {
            using CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromSeconds(15));
            ApiResponse<LoginResponse> response = await authApi.LoginAsync(credentials, cancellationTokenSource.Token);

            if (response.IsSuccessStatusCode)
            {
                string? token = response.Content?.Token;
                await localStorage.SetItemAsync(AuthLocalStorageKey, token, cancellationTokenSource.Token);
                NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
                return new AuthResult { Succeeded = true };
            }
        }
        catch
        {
            //Logging
        }

        return new AuthResult
        {
            Succeeded = false,
            ErrorList = ["Invalid email or password"]
        };
    }

    public async Task LogoutAsync()
    {
        await localStorage.RemoveItemAsync(AuthLocalStorageKey);
        NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
    }
}