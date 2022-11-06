using Notescrib.Api.Application.Users.Models;

namespace Notescrib.Api.Application.Auth.Models;

public class TokenResponse
{
    public string Token { get; set; } = null!;
    public UserDetails User { get; set; } = null!;
}
