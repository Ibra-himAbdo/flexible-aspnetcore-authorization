namespace FlexibleAuthorization.Client.Pages.Roles;

public partial class Index
{
    [Inject] public IRolesApi? RolesApi { get; set; }

    public ICollection<RoleDto> Roles { get; set; } = new List<RoleDto>();
    private string _newRoleName = string.Empty;
    private RoleDto? _roleToEdit;
    private string? _errorMessage;

    protected override async Task OnInitializedAsync()
    {
        await LoadRolesAsync();
    }

    private async Task LoadRolesAsync()
    {
        _errorMessage = string.Empty;
        if (RolesApi is null) return;

        try
        {
            using CancellationTokenSource cts = new(TimeSpan.FromSeconds(15));
            ApiResponse<IEnumerable<RoleDto>> response = await RolesApi.GetRolesAsync(cts.Token);

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                Roles = response.Content.ToList();
            }
            else
            {
                _errorMessage = "Failed to load roles.";
            }
        }
        catch (Exception ex)
        {
            _errorMessage = $"Error: {ex.Message}";
        }
    }

    private async Task AddRole()
    {
        _errorMessage = string.Empty;
        if (string.IsNullOrWhiteSpace(_newRoleName) || RolesApi is null) return;

        try
        {
            using CancellationTokenSource cts = new(TimeSpan.FromSeconds(15));
            ApiResponse<RoleDto> response = await RolesApi.CreateRoleAsync(
                new RoleDto("", _newRoleName, Permissions.None), cts.Token);

            if (response.IsSuccessStatusCode && response.Content != null)
            {
                Roles.Add(response.Content);
                _newRoleName = string.Empty;
            }
            else
            {
                _errorMessage = "Failed to create role.";
            }
        }
        catch (Exception ex)
        {
            _errorMessage = $"Error: {ex.Message}";
        }
    }

    private void EditRole(RoleDto role) => _roleToEdit = role;

    private void CancelEditRole() => _roleToEdit = null;

    private async Task UpdateRole()
    {
        if (RolesApi is null || _roleToEdit == null) return;

        _errorMessage = string.Empty;

        try
        {
            using CancellationTokenSource cts = new(TimeSpan.FromSeconds(15));
            ApiResponse<object> response = await RolesApi.UpdateRoleAsync(_roleToEdit.Id, _roleToEdit, cts.Token);

            if (!response.IsSuccessStatusCode)
            {
                _errorMessage = "Failed to update role.";
            }

            _roleToEdit = null;
        }
        catch (Exception ex)
        {
            _errorMessage = $"Error: {ex.Message}";
        }
    }

    private async Task DeleteRole(RoleDto role)
    {
        if (RolesApi is null) return;

        _errorMessage = string.Empty;

        try
        {
            using CancellationTokenSource cts = new(TimeSpan.FromSeconds(15));
            ApiResponse<object> response = await RolesApi.DeleteRoleAsync(role.Id, cts.Token);

            if (response.IsSuccessStatusCode)
            {
                Roles.Remove(role);
            }
            else
            {
                _errorMessage = "Failed to delete role.";
            }
        }
        catch (Exception ex)
        {
            _errorMessage = $"Error: {ex.Message}";
        }
    }
}