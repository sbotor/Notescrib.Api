﻿using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notescrib.Core.Cqrs.Behaviors;
using Notescrib.Core.Extensions;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Identity.Clients;
using Notescrib.Identity.Clients.Config;
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
        services.AddSingleton<IDateTimeProvider, UtcDateTimeProvider>();
        
        services.AddPipelineBehavior(typeof(LoggingBehavior<,>))
            .AddPipelineBehavior(typeof(ValidationBehavior<,>));
        services.AddMediatrWithValidation(ThisAssembly);

        services.AddTransient<IUserMapper, UserMapper>();

        services.AddScoped<IUserContextProvider, UserContextProvider>();
        services.AddHttpContextAccessor();
        
        services.AddIdentityServices(config);
        
        services.AddNotesIntegration(config);
        services.AddEmailsIntegration(config);
        
        return services;
    }

    private static void AddIdentityServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddDbContext<UserDbContext>(options =>
        {
            options.UseNpgsql(config.GetConnectionString(UserDbContext.DefaultConStrName));
        });
        
        services.AddIdentityCore<AppUser>(ConfigureIdentityOptions)
            .AddEntityFrameworkStores<UserDbContext>()
            .AddUserManager<AppUserManager>()
            .AddDefaultTokenProviders();
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

    private static void AddNotesIntegration(this IServiceCollection services, IConfiguration config)
    {
        var section = config.GetSection(nameof(NotesApiSettings));
        services.Configure<NotesApiSettings>(section);

        services.AddScoped<INotesApiClient, NotesApiClient>();
        
        services.AddHttpClient(nameof(NotesApiClient), client =>
        {
            var settings = section.Get<NotesApiSettings>()!;
            client.BaseAddress = settings.BaseUrl;
        });
    }

    private static void AddEmailsIntegration(this IServiceCollection services, IConfiguration config)
    {
        var section = config.GetSection(nameof(EmailsApiSettings));
        services.Configure<EmailsApiSettings>(section);

        services.AddScoped<IEmailsApiClient, EmailsApiClient>();
        
        services.AddHttpClient(nameof(EmailsApiClient), client =>
        {
            var settings = section.Get<EmailsApiSettings>()!;
            client.BaseAddress = settings.BaseUrl;
            client.DefaultRequestHeaders.Add("X-Api-Key", settings.ApiKey);
        });
    }
}
