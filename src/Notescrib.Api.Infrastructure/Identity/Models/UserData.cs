using Microsoft.AspNetCore.Identity;

namespace Notescrib.Api.Infrastructure.Identity.Models;

internal class UserData : IdentityUser
{
    public bool IsActive { get; set; }
}
