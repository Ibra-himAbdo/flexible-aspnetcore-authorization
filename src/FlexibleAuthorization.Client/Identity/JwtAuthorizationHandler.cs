namespace FlexibleAuthorization.Client;

public class JwtAuthorizationHandler(ILocalStorageService localStorage) : DelegatingHandler
{
    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        string? token =
            await localStorage.GetItemAsync<string>(JwtAuthenticationStateProvider.AuthLocalStorageKey,
                cancellationToken);
        if (!string.IsNullOrEmpty(token))
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

        return await base.SendAsync(request, cancellationToken);
    }
}