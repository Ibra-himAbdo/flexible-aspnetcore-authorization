namespace FlexibleAuthorization.Shared;

public static class PolicyNameHelper
{
    public const string Prefix = "Permissions";

    public static bool IsValidPolicyName(string? policyName)
        => policyName is not null &&
           policyName.StartsWith(Prefix, StringComparison.OrdinalIgnoreCase);

    public static string GeneratePolicyNameFor(Permissions permissions) 
        => $"{Prefix}{(int)permissions}";

    public static Permissions GetPermissionsFrom(string policyName)
    {
        int permissionsValue = int.Parse(policyName[Prefix.Length..]!);
        return (Permissions)permissionsValue;
    }
}