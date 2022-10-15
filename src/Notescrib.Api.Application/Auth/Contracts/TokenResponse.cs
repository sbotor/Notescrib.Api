using Notescrib.Api.Application.Users.Models;

namespace Notescrib.Api.Application.Auth.Contracts;

public class TokenResponse
{
    public string Token { get; set; } = null!;
    public UserDetails User { get; set; } = null!;
}
