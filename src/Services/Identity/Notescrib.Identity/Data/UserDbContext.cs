using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Notescrib.Identity.Features.Users;

namespace Notescrib.Identity.Data;

public class UserDbContext : IdentityDbContext<AppUser>
{
    public const string DefaultConStrName = "NotescribUserDb";
    
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }
}
