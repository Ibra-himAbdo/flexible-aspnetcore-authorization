namespace FlexibleAuthorization.Client;

public interface IRolesApi
{
    [Get("/api/Admin/Roles")]
    Task<ApiResponse<IEnumerable<RoleDto>>> GetRolesAsync();

    [Post("/api/Admin/Roles")]
    Task<ApiResponse<RoleDto>> CreateRoleAsync([Body] RoleDto newRole);

    [Put("/api/Admin/Roles/{id}")]
    Task<ApiResponse<object>> UpdateRoleAsync(string id, [Body] RoleDto updatedRole);

    [Delete("/api/Admin/Roles/{id}")]
    Task<ApiResponse<object>> DeleteRoleAsync(string id);
}