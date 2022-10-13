using Notescrib.Api.Application.Auth.Queries;

namespace Notescrib.Api.Application.Contracts.User;

public class LoginRequest
{
    public string Email { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;

    public Authenticate.Query ToQuery()
        => new(Email, Password);
}
