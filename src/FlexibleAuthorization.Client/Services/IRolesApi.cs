namespace FlexibleAuthorization.Client;

public interface IRolesApi : IFlexibleAuthorizationApi
{
    [Get("/api/Admin/Roles")]
    Task<ApiResponse<IEnumerable<RoleDto>>> GetRolesAsync(CancellationToken cancellationToken);

    [Post("/api/Admin/Roles")]
    Task<ApiResponse<RoleDto>> CreateRoleAsync([Body] RoleDto newRole, CancellationToken cancellationToken);

    [Put("/api/Admin/Roles/{id}")]
    Task<ApiResponse<object>> UpdateRoleAsync(string id, [Body] RoleDto updatedRole,
        CancellationToken cancellationToken);

    [Delete("/api/Admin/Roles/{id}")]
    Task<ApiResponse<object>> DeleteRoleAsync(string id, CancellationToken cancellationToken);
}