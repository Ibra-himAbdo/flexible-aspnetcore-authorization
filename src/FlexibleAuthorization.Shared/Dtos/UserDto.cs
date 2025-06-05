namespace FlexibleAuthorization.Shared.Dtos;

public class UserDto
{
    public UserDto() : this(string.Empty, string.Empty, string.Empty) { }

    public UserDto(string id, string userName, string email)
    {
        Id = id;
        UserName = userName;
        Email = email;
    }

    public UserDto(string id, string userName, string email, List<string> roles)
    {
        Id = id;
        UserName = userName;
        Email = email;
        Roles = roles ?? [];
    }

    public string Id { get; set; }

    public string UserName { get; set; }

    public string Email { get; set; }

    public List<string> Roles { get; set; } = [];
}