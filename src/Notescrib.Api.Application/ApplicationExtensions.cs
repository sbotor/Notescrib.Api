using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notescrib.Api.Application.Configuration;
using Notescrib.Api.Application.Extensions;
using Notescrib.Api.Application.Mappers;
using Notescrib.Api.Application.Services;

namespace Notescrib.Api.Application;

public static class ApplicationExtensions
{
    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.ConfigureSettings<JwtSettings>(config);

        services.AddHttpContextAccessor();

        services
            .AddScoped<IWorkspaceService, WorkspaceService>()
            .AddScoped<IWorkspaceMapper, WorkspaceMapper>();

        services
            .AddScoped<IPermissionService, MockPermissionService>()
            .AddScoped<IUserContextService, UserContextService>();

        services
            .AddScoped<IAuthService, AuthService>()
            .AddScoped<IJwtProvider, JwtProvider>();

        return services;
    }
}
