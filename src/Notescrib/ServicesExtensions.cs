using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notescrib.Contracts;
using Notescrib.Core.Cqrs.Behaviors;
using Notescrib.Core.Extensions;
using Notescrib.Core.Services;
using Notescrib.Data;
using Notescrib.Features.Notes.Mappers;
using Notescrib.Services;
using Notescrib.Utils.Mediatr;

namespace Notescrib;

public static class ServicesExtensions
{
    private static readonly Assembly ThisAssembly = typeof(ServicesExtensions).Assembly;
    
    public static IServiceCollection AddRequiredServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddMediatR();

        var connectionStr = config.GetConnectionString("Default")
            ?? throw new InvalidOperationException("No DB connection string.");
        services.AddDbContext<NotescribDbContext>(x => x.UseNpgsql(connectionStr));
        
        services
            .AddHttpContextAccessor()
            .AddScoped<IPermissionGuard, PermissionGuard>()
            .AddScoped<IUserContext, NotesUserContext>();

        services.AddAll(typeof(IMapper<,>));
        services.AddScoped<INoteDetailsMapper, NoteDetailsMapper>()
            .AddScoped<INoteOverviewMapper, NoteOverviewMapper>();

        services.AddSingleton<IClock, UtcClock>();
        
        return services;
    }

    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddPipelineBehavior(typeof(LoggingBehavior<,>))
            .AddPipelineBehavior(typeof(PagingValidationBehavior<,>));
        services.AddMediatrWithValidation(ThisAssembly);

        return services;
    }

    private static IServiceCollection AddAll(this IServiceCollection services,
        Type parentType,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        => services.Scan(scan
            => scan.FromAssemblies(ThisAssembly)
                .AddClasses(classes => classes.AssignableTo(parentType))
                .AsImplementedInterfaces()
                .WithLifetime(lifetime));
}
