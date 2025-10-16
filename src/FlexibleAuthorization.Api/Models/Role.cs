namespace FlexibleAuthorization.Api;

public class Role : IdentityRole
{
    public Permissions Permissions { get; set; }
}