using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace Notescrib.Api.Infrastructure.Identity;

internal class UserDbContextFactory : IDesignTimeDbContextFactory<UserDbContext>
{
    private const string DefaultConnection = @"Data Source=(localdb)\MSSQLLocalDB;Initial Catalog=UserDb;Integrated Security=True;Connect Timeout=30;Encrypt=False;TrustServerCertificate=False;ApplicationIntent=ReadWrite;MultiSubnetFailover=False";

    public UserDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<UserDbContext>()
            .UseSqlServer(DefaultConnection)
            .Options;

        return new UserDbContext(options);
    }
}
