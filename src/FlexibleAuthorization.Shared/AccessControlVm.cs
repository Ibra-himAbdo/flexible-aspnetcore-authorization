namespace FlexibleAuthorization.Shared;

public class AccessControlVm
{
    internal AccessControlVm()
    {
    }

    public AccessControlVm(List<RoleDto> roles)
    {
        Roles = roles;

        foreach (var permission in
                 PermissionsProvider.GetAll()
                     .Where(permission => permission != Permissions.None))
        {
            AvailablePermissions.Add(permission);
        }
    }

    public List<RoleDto> Roles { get; set; } = [];

    public List<Permissions> AvailablePermissions { get; set; } = [];
}