using Notescrib.Identity.Users.Models;

namespace Notescrib.Identity.Auth.Models;

public class TokenResponse
{
    public string Token { get; set; } = null!;
    public UserDetails User { get; set; } = null!;
}
