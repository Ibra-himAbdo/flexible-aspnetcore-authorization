namespace FlexibleAuthorization.Api.Models;

public class User : IdentityUser
{
    public string? Name { get; set; }
}