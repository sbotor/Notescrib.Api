using Notescrib.Identity.Features.Users.Models;

namespace Notescrib.Identity.Features.Auth.Models;

public class TokenResponse
{
    public string Token { get; set; } = null!;
    public UserDetails User { get; set; } = null!;
}
