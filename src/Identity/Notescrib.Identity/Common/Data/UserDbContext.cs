using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Notescrib.Identity.Common.Entities;

namespace Notescrib.Identity.Common.Data;

internal class UserDbContext : IdentityDbContext<AppUser>
{
    internal const string DefaultConStrName = "UserDb";
    
    public UserDbContext(DbContextOptions<UserDbContext> options)
        : base(options)
    {
    }

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
}
