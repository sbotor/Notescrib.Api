using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notescrib.Core.Cqrs.Behaviors;
using Notescrib.Core.Extensions;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Identity.Data;
using Notescrib.Identity.Features.Auth.Queries;
using Notescrib.Identity.Features.Users;
using Notescrib.Identity.Features.Users.Mappers;

namespace Notescrib.Identity;

public static class ServicesExtensions
{
    private static readonly Assembly ThisAssembly = typeof(Authenticate).Assembly;
    
    public static IServiceCollection AddRequiredServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddMediatrWithValidation(ThisAssembly)
            .AddPipelineBehavior(typeof(LoggingBehavior<,>))
            .AddPipelineBehavior(typeof(ValidationBehavior<,>));

        services.AddTransient<IUserMapper, UserMapper>();
        
        services.AddIdentityServices(config);
        
        return services;
    }

    private static void AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseSqlServer(config.GetConnectionString(UserDbContext.DefaultConStrName));
        });

        services.AddIdentityCore<AppUser>(ConfigureIdentityOptions)
            .AddEntityFrameworkStores<UserDbContext>()
            .AddUserManager<AppUserManager>();
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
