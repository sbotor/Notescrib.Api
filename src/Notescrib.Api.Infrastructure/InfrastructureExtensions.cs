using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notescrib.Api.Application.Extensions;
using Notescrib.Api.Application.Repositories;
using Notescrib.Api.Application.Services;
using Notescrib.Api.Infrastructure.Identity;
using Notescrib.Api.Infrastructure.Repositories;
using Notescrib.Api.Infrastructure.Services;

namespace Notescrib.Api.Infrastructure;

public static class InfrastructureExtensions
{
    private const string UserDbName = "UserDb";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.Configure(config);

        services.AddSingleton<IMongoCollectionProvider, MongoCollectionProvider>();
        services.AddScoped(typeof(IMongoPersistenceProvider<>), typeof(MongoPersistenceProvider<>));

        services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();

        services.AddIdentity(config);

        return services;
    }

    private static IServiceCollection Configure(this IServiceCollection services, IConfiguration config)
    {
        services.ConfigureSettings<MongoDbSettings>(config);

        return services;
    }

    private static IServiceCollection AddIdentity(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString(UserDbName));
        });

        services.AddIdentityCore<IdentityUser>(options => ConfigureIdentityOptions(options))
            .AddEntityFrameworkStores<UserDbContext>();
        //.AddClaimsPrincipalFactory<ClaimsPrincipalFactory>(); // TODO: See if it works without it.

        services.AddScoped<IUserService, UserService>();

        return services;
    }

    private static void ConfigureIdentityOptions(IdentityOptions options)
    {
        options.SignIn.RequireConfirmedEmail = true;

        options.User.RequireUniqueEmail = true;

        options.Password = new PasswordOptions
        {
            RequireDigit = true,
            RequireLowercase = true,
            RequireUppercase = true,
            RequireNonAlphanumeric = true,
            RequiredLength = 6
        };
    }
}
