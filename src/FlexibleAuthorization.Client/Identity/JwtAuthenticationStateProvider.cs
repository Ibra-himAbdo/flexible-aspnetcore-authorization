namespace FlexibleAuthorization.Client;

public class JwtAuthenticationStateProvider(IAuthApi authApi, ILocalStorageService localStorage)
    : AuthenticationStateProvider, IJwtAuthenticationStateProvider
{
    private const string AuthLocalStorageKey = "auth_token";
    private readonly ClaimsPrincipal _unauthenticated = new(new ClaimsIdentity());

    private readonly JsonSerializerOptions _jsonSerializerOptions = new()
    {
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    public override async Task<AuthenticationState> GetAuthenticationStateAsync()
    {
        ClaimsPrincipal user = _unauthenticated;

        try
        {
            string? token = await localStorage.GetItemAsync<string>(AuthLocalStorageKey);
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
            ApiResponse<LoginResponse> response = await authApi.LoginAsync(credentials);

            if (response.IsSuccessStatusCode)
            {
                string? token = response.Content?.Token;
                await localStorage.SetItemAsync(AuthLocalStorageKey, token);
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