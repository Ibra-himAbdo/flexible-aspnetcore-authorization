namespace FlexibleAuthorization.Shared;

public class AccessControlViewModel
{
    internal AccessControlViewModel()
    {
    }

    public AccessControlViewModel(List<RoleDto> roles)
    {
        Roles = roles;

        foreach (Permissions permission in
                 PermissionsProvider.GetAll()
                     .Where(permission => permission != Permissions.None))
        {
            AvailablePermissions.Add(permission);
        }
    }

    public List<RoleDto> Roles { get; set; } = [];

    public List<Permissions> AvailablePermissions { get; set; } = [];
}