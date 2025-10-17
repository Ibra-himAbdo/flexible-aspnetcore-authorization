namespace FlexibleAuthorization.Client;

public interface IUsersApi
{
    [Get("/api/Admin/Users")]
    Task<ApiResponse<IEnumerable<UserDto>>> GetUsersAsync();

    [Get("/api/Admin/Users/{id}")]
    Task<ApiResponse<UserDto>> GetUserByIdAsync(string id);

    [Put("/api/Admin/Users/{id}")]
    Task<ApiResponse<object>> UpdateUserAsync(string id, [Body] UserDto updatedUser);
}