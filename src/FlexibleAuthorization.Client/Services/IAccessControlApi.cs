namespace FlexibleAuthorization.Client;

public interface IAccessControlApi : IFlexibleAuthorizationApi
{
    [Get("/api/Admin/AccessControl")]
    Task<ApiResponse<AccessControlViewModel>> GetConfigurationAsync();

    [Put("/api/Admin/AccessControl")]
    Task<ApiResponse<object>> UpdateConfigurationAsync([Body] RoleDto updatedRole);
}