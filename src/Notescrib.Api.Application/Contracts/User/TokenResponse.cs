namespace Notescrib.Api.Application.Contracts.User;

public class TokenResponse
{
    public string? Token { get; set; }
    public UserDetails? User { get; set; }
}
