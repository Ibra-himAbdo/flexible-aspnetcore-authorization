namespace FlexibleAuthorization.Client.Pages;

public partial class Login
{
    [SupplyParameterFromForm] private LoginModel? Model { get; set; }

    private string[] _errorList = [];
    private bool _loading;
    protected override void OnInitialized() => Model ??= new LoginModel();

    private async Task Submit()
    {
        _loading = true;

        AuthResult result = await CookieAuthenticationStateProvider.LoginAsync(Model!);

        if (result.Succeeded)
        {
            Navigation.NavigateTo("/");
        }
        else
        {
            _errorList = result.ErrorList;
        }

        _loading = false;
    }
}