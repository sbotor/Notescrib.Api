namespace Notescrib.Identity.Features.Users.Models;

public class UserDetails
{
    public string Id { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public bool IsActive { get; set; }
}
