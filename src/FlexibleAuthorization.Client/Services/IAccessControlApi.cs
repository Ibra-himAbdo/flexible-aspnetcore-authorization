namespace FlexibleAuthorization.Client;

public interface IAccessControlApi
{
    [Get("/api/Admin/AccessControl")]
    Task<ApiResponse<AccessControlVm>> GetConfigurationAsync();

    [Put("/api/Admin/AccessControl")]
    Task<ApiResponse<object>> UpdateConfigurationAsync([Body] RoleDto updatedRole);
}