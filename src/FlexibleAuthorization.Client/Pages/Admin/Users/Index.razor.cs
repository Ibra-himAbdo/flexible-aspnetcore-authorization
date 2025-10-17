namespace FlexibleAuthorization.Client.Pages.Users;

public partial class Index
{
    [Inject] public IUsersApi? UsersClient { get; set; }

    public ICollection<UserDto> Users { get; set; } = new List<UserDto>();

    public string ErrorMessage { get; set; } = string.Empty;

    protected override async Task OnInitializedAsync()
    {
        ErrorMessage = string.Empty;

        if (UsersClient == null)
        {
            ErrorMessage = "Missing required services.";
            return;
        }

        using CancellationTokenSource cancellationTokenSource = new CancellationTokenSource(TimeSpan.FromSeconds(15));

        try
        {
            ApiResponse<IEnumerable<UserDto>> usersResponse =
                await UsersClient.GetUsersAsync(cancellationTokenSource.Token);

            if (usersResponse.IsSuccessStatusCode && usersResponse.Content != null)
            {
                Users = usersResponse.Content.ToList();
            }
            else
            {
                Users = new List<UserDto>();
                ErrorMessage = "Failed to load user data.";
            }
        }
        catch (Exception exception)
        {
            Users = new List<UserDto>();
            ErrorMessage = $"An error occurred while loading users: {exception.Message}";
        }
    }
}