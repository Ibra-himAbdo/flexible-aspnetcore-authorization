namespace FlexibleAuthorization.Client;

public interface IAuthApi
{
    [Post("/api/Auth/login")]
    Task<ApiResponse<LoginResponse>> LoginAsync([Body] LoginModel model);
}