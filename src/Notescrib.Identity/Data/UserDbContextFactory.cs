using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Notescrib.Identity.Data;

internal class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    private const string DefaultConnection = @"User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=NotescribUserDb;";

    public UserDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseNpgsql(DefaultConnection)
            .Options;

        return new UserDbContext(options);
    }
}
