using System.Reflection;
using System.Runtime.CompilerServices;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notescrib.Api.Application.Auth.Services;
using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Common.Configuration;
using Notescrib.Api.Application.Cqrs.Behaviors;
using Notescrib.Api.Application.Extensions;
using Notescrib.Api.Application.Notes.Mappers;
using Notescrib.Api.Application.Users.Mappers;
using Notescrib.Api.Application.Workspaces.Mappers;

[assembly: InternalsVisibleTo("Notescrib.Api.Application.Tests")]

namespace Notescrib.Api.Application;

public static class ApplicationExtensions
{
    private static readonly Assembly ThisAssembly = typeof(ApplicationExtensions).Assembly;

    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services
            .Configure(config)
            .AddMappers();

        services.AddHttpContextAccessor();

        services
            .AddGeneralServices()
            .AddMediatR();

        services.AddValidatorsFromAssembly(ThisAssembly, includeInternalTypes: true);

        return services;
    }

    private static IServiceCollection Configure(this IServiceCollection services, IConfiguration config)
    {
        services.ConfigureSettings<JwtSettings>(config);

        return services;
    }

    private static IServiceCollection AddGeneralServices(this IServiceCollection services)
    {
        services
            .AddScoped<ISharingService, SharingService>()
            .AddScoped<IUserContextProvider, UserContextProvider>()
            .AddScoped<IJwtProvider, JwtProvider>();

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }

    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(PagingValidationBehavior<,>))
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddMediatR(ThisAssembly);

        return services;
    }

    private static IServiceCollection AddMappers(this IServiceCollection services)
    {
        services
            .AddSingleton<IWorkspaceMapper, WorkspaceMapper>()
            .AddSingleton<IFolderMapper, FolderMapper>()
            .AddSingleton<IUserMapper, UserMapper>()
            .AddSingleton<INoteMapper, NoteMapper>();
        
        return services;
    }
}
