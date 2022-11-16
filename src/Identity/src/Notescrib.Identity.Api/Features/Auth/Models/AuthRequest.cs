using Notescrib.Identity.Features.Auth.Queries;

namespace Notescrib.Identity.Api.Features.Auth.Models;

public class AuthRequest
{
    public string Email { get; set; } = null!;
    public string Password { get; set; } = null!;

    public Authenticate.Query ToQuery()
        => new(Email, Password);
}
