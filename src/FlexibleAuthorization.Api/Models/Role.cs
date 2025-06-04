namespace FlexibleAuthorization.Api.Models;

public class Role : IdentityRole
{
    public Permissions Permissions { get; set; }
}