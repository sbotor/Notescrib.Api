using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Notescrib.Identity.Features.Users;

namespace Notescrib.Identity.Data;

public class UserDbContext : IdentityDbContext<AppUser>
{
    public const string DefaultConStrName = "NotescribUserDb";
    
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        builder.Entity<AppUser>()
            .HasQueryFilter(x => x.IsActive);
    }
}
