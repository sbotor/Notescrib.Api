using Microsoft.AspNetCore.Identity;

namespace Notescrib.Identity.Features.Users;

public class AppUser : IdentityUser
{
    public bool IsActive { get; set; }
}
