namespace FlexibleAuthorization.Client.Pages.AccessControl;

public partial class Index
{
    [Inject] private IAccessControlApi? AccessControlApi { get; set; }

    private AccessControlViewModel? _accessControlVm;
    private string? _errorMessage;
    private bool _isLoading;

    protected override async Task OnInitializedAsync()
    {
        if (AccessControlApi is null)
        {
            _errorMessage = "Access control service is not available.";
            return;
        }

        try
        {
            _isLoading = true;
            ApiResponse<AccessControlViewModel> response = await AccessControlApi.GetConfigurationAsync();

            if (response.IsSuccessStatusCode)
            {
                _accessControlVm = response.Content;
            }
            else
            {
                _errorMessage = $"Failed to load access control configuration. " +
                                $"Server returned: {response.StatusCode}";
            }
        }
        catch (Exception ex)
        {
            _errorMessage = $"An unexpected error occurred: {ex.Message}";
        }
        finally
        {
            _isLoading = false;
        }
    }

    private async Task Set(RoleDto role, Permissions permission, bool granted)
    {
        if (AccessControlApi is null)
        {
            _errorMessage = "Access control service is not available.";
            return;
        }

        try
        {
            role.Set(permission, granted);
            var response = await AccessControlApi.UpdateConfigurationAsync(role);

            if (!response.IsSuccessStatusCode)
            {
                _errorMessage = $"Failed to update role '{role.Name}'. Server returned {response.StatusCode}.";
            }
        }
        catch (Exception ex)
        {
            _errorMessage = $"An error occurred while updating role '{role.Name}': {ex.Message}";
        }
    }
}