namespace FlexibleAuthorization.Shared.Authorization;

public static class PermissionsProvider
{
    public static List<Permissions> GetAll()
        => Enum.GetValues<Permissions>()
            .ToList();
}