namespace FlexibleAuthorization.Api;

public class User : IdentityUser
{
    public string? Name { get; set; }
}