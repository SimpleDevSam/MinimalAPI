namespace MinimalAPI_KeyCloack.Application.Requests;

public class CreateUserRequest
{
    public string UserName { get; set; } = string.Empty;
    public string UserEmail { get; set; } = string.Empty;
}
