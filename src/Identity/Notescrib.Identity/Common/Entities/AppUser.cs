using Microsoft.AspNetCore.Identity;

namespace Notescrib.Identity.Common.Entities;

public class AppUser : IdentityUser
{
    public bool IsActive { get; set; }
}
