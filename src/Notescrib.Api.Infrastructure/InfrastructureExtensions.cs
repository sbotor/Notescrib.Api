using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notescrib.Api.Application.Auth.Services;
using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Extensions;
using Notescrib.Api.Application.Notes;
using Notescrib.Api.Application.Users;
using Notescrib.Api.Application.Workspaces;
using Notescrib.Api.Infrastructure.Identity;
using Notescrib.Api.Infrastructure.Identity.Models;
using Notescrib.Api.Infrastructure.MongoDb;
using Notescrib.Api.Infrastructure.MongoDb.Providers;
using Notescrib.Api.Infrastructure.MongoDb.Repositories;

namespace Notescrib.Api.Infrastructure;

public static class InfrastructureExtensions
{
    private const string UserDbName = "UserDb";

    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.Configure(config);

        services.AddSingleton<IMongoCollectionProvider, MongoCollectionProvider>();

        services
            .AddScoped(typeof(IRepository<>), typeof(MongoRepository<>))
            .AddScoped(typeof(IMongoRepository<>), typeof(MongoRepository<>))
            .AddScoped<IFolderRepository, FolderRepository>()
            .AddScoped<IWorkspaceRepository, WorkspaceRepository>()
            .AddScoped<INoteRepository, NoteRepository>();

        services.AddIdentity(config);

        services.AddAutoMapper(typeof(MappingProfile));

        return services;
    }

    public static IApplicationBuilder MigrateDatabase(this IApplicationBuilder builder)
    {
        using var scope = builder.ApplicationServices.CreateScope();
        using var context = scope.ServiceProvider.GetService<UserDbContext>();
        
        if (context != null)
        {
            context.Database.Migrate();
        }

        return builder;
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

        services.AddIdentityCore<UserData>(options => ConfigureIdentityOptions(options))
            .AddEntityFrameworkStores<UserDbContext>();

        services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IUserRepository, UserRepository>();

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
