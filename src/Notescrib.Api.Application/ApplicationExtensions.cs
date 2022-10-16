using System.Reflection;
using System.Runtime.CompilerServices;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notescrib.Api.Application.Auth.Services;
using Notescrib.Api.Application.Common.Configuration;
using Notescrib.Api.Application.Common.Services;
using Notescrib.Api.Application.Cqrs.Behaviors;
using Notescrib.Api.Application.Extensions;

[assembly: InternalsVisibleTo("Notescrib.Api.Application.Tests")]

namespace Notescrib.Api.Application;

public static class ApplicationExtensions
{
    private static readonly Assembly ThisAssembly = typeof(ApplicationExtensions).Assembly;

    public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration config)
    {
        services.ConfigureSettings<JwtSettings>(config);

        services.AddHttpContextAccessor();

        services
            .AddScoped<IPermissionService, PermissionService>()
            .AddScoped<IUserContextService, UserContextService>()
            .AddScoped<IJwtProvider, JwtProvider>();

        services.AddMediatR(ThisAssembly);
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(PagingValidationBehavior<,>));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddValidatorsFromAssembly(ThisAssembly, includeInternalTypes: true);

        services.AddAutoMapper(ThisAssembly);

        return services;
    }
}
