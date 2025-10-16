namespace FlexibleAuthorization.Client.Shared;

public class FlexibleAuthorizeView : AuthorizeView
{
    [Parameter]
    public Permissions Permissions
    {
        get => string.IsNullOrEmpty(Policy) ? Permissions.None : PolicyNameHelper.GetPermissionsFrom(Policy);
        set => Policy = PolicyNameHelper.GeneratePolicyNameFor(value);
    }
}