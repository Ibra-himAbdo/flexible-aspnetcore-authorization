namespace FlexibleAuthorization.Client;

public interface IAuthApi : IFlexibleAuthorizationApi
{
    [Post("/api/Auth/login")]
    Task<ApiResponse<LoginResponse>> LoginAsync([Body] LoginModel model,
        CancellationToken cancellationToken);
}