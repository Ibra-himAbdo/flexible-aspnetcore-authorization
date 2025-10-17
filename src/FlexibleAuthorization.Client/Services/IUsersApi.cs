namespace FlexibleAuthorization.Client;

public interface IUsersApi : IFlexibleAuthorizationApi
{
    [Get("/api/Admin/Users")]
    Task<ApiResponse<IEnumerable<UserDto>>> GetUsersAsync(CancellationToken cancellationToken);

    [Get("/api/Admin/Users/{id}")]
    Task<ApiResponse<UserDto>> GetUserByIdAsync(string id, CancellationToken cancellationToken);

    [Put("/api/Admin/Users/{id}")]
    Task<ApiResponse<object>> UpdateUserAsync(string id, [Body] UserDto updatedUser,
        CancellationToken cancellationToken);
}