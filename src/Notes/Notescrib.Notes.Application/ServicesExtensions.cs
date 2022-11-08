using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notescrib.Notes.Application.Common.Mediatr;
using Notescrib.Notes.Application.Common.Providers;
using Notescrib.Notes.Application.Notes.Mappers;
using Notescrib.Notes.Application.Workspaces.Mappers;

namespace Notescrib.Notes.Application;

public static class ServicesExtensions
{
    private static readonly Assembly ThisAssembly = typeof(ServicesExtensions).Assembly;

    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddMappers();

        services.AddHttpContextAccessor();

        services
            .AddGeneralServices()
            .AddMediatR();

        services.AddValidatorsFromAssembly(ThisAssembly, includeInternalTypes: true);

        return services;
    }

    private static IServiceCollection AddGeneralServices(this IServiceCollection services)
    {
        services
            .AddScoped<ISharingGuard, SharingGuard>()
            .AddScoped<IUserContextProvider, UserContextProvider>();

        services.AddSingleton<IDateTimeProvider, DateTimeProvider>();

        return services;
    }

    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services
            .AddScoped(typeof(IPipelineBehavior<,>), typeof(PagingValidationBehavior<,>));

        services.AddMediatR(ThisAssembly);

        return services;
    }

    private static IServiceCollection AddMappers(this IServiceCollection services)
    {
        services
            .AddSingleton<IWorkspaceMapper, WorkspaceMapper>()
            .AddSingleton<IFolderMapper, FolderMapper>()
            .AddSingleton<INoteMapper, NoteMapper>();
        
        return services;
    }
}
