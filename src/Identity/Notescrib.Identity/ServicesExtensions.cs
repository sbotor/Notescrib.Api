﻿using System.Reflection;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notescrib.Identity.Data;
using Notescrib.Identity.Features.Auth.Providers;
using Notescrib.Identity.Features.Auth.Queries;
using Notescrib.Identity.Features.Users;
using Notescrib.Identity.Features.Users.Mappers;

namespace Notescrib.Identity;

public static class ServicesExtensions
{
    private static readonly Assembly ThisAssembly = typeof(Authenticate).Assembly;
    
    public static IServiceCollection AddServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddMediatR(ThisAssembly);
        services.AddFluentValidation(new[] { ThisAssembly });

        services.AddTransient<IJwtProvider, JwtProvider>();

        services.AddTransient<IUserMapper, UserMapper>();
        
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
