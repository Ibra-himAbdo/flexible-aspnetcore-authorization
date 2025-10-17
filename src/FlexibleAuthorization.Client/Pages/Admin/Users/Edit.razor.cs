namespace FlexibleAuthorization.Client.Pages.Users;

public partial class Edit
{
    [Parameter] public string? UserId { get; set; }

    [Inject] public IUsersApi? UsersClient { get; set; }

    [Inject] public IRolesApi? RolesClient { get; set; }

    [Inject] public NavigationManager? Navigation { get; set; }

    public UserDto User { get; set; } = new UserDto();

    public ICollection<RoleDto> Roles { get; set; } = new List<RoleDto>();

    private string? _errorMessage;

    protected override async Task OnParametersSetAsync()
    {
        _errorMessage = string.Empty;

        if (RolesClient == null || UsersClient == null || string.IsNullOrWhiteSpace(UserId))
        {
            _errorMessage = "Missing required services or user ID.";
            return;
        }

        using CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromSeconds(15));

        try
        {
            ApiResponse<IEnumerable<RoleDto>> rolesResponse =
                await RolesClient.GetRolesAsync(cancellationTokenSource.Token);

            if (rolesResponse.IsSuccessStatusCode && rolesResponse.Content != null)
            {
                Roles = rolesResponse.Content.ToList();
            }
            else
            {
                Roles = new List<RoleDto>();
                _errorMessage = "Failed to load roles.";
            }

            ApiResponse<UserDto> usersResponse =
                await UsersClient.GetUserByIdAsync(UserId, cancellationTokenSource.Token);

            if (usersResponse.IsSuccessStatusCode && usersResponse.Content != null)
            {
                User = usersResponse.Content;
            }
            else
            {
                User = new UserDto();
                _errorMessage = "Failed to load user data.";
            }
        }
        catch (Exception exception)
        {
            _errorMessage = $"An error occurred: {exception.Message}";
        }
    }

    public void ToggleSelectedRole(string roleName)
    {
        if (User.Roles.Contains(roleName))
        {
            User.Roles.Remove(roleName);
        }
        else
        {
            User.Roles.Add(roleName);
        }

        StateHasChanged();
    }

    public async Task UpdateUser()
    {
        _errorMessage = string.Empty;

        if (UsersClient == null || Navigation == null)
        {
            _errorMessage = "User API client or Navigation manager not available.";
            return;
        }

        using CancellationTokenSource cancellationTokenSource = new(TimeSpan.FromSeconds(15));

        try
        {
            ApiResponse<object> updateResponse =
                await UsersClient.UpdateUserAsync(User.Id, User, cancellationTokenSource.Token);

            if (updateResponse.IsSuccessStatusCode)
            {
                Navigation.NavigateTo("/admin/users");
            }
            else
            {
                _errorMessage = "Failed to update user.";
            }
        }
        catch (Exception exception)
        {
            _errorMessage = $"An error occurred while updating: {exception.Message}";
        }
    }

    private void NavigateBack()
    {
        Navigation?.NavigateTo("/admin/users");
    }
}