namespace FlexibleAuthorization.Client;

public class AuthResult
{
    public bool Succeeded { get; init; }
    public string[] ErrorList { get; set; } = [];
}