using Microsoft.Extensions.DependencyInjection;
using Notescrib.Api.Application.Services;

namespace Notescrib.Api.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services)
    {
        services
            .AddScoped<INoteService, NoteService>()
            .AddScoped<IWorkspaceService, WorkspaceService>();

        services
            .AddScoped<IPermissionService, MockPermissionService>()
            .AddScoped<IUserContextService, UserContextService>();

        return services;
    }
}
