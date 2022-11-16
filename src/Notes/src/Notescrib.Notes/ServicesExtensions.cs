using System.Reflection;
using System.Runtime.CompilerServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Notescrib.Core.Extensions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Models.Configuration;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils.Mediatr;
using Notescrib.Notes.Utils.MongoDb;

[assembly: InternalsVisibleTo("Notescrib.Notes.Tests")]

namespace Notescrib.Notes;

public static class ServicesExtensions
{
    private static readonly Assembly ThisAssembly = typeof(ServicesExtensions).Assembly;
    
    public static IServiceCollection AddRequiredServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddMongoDb(config);

        services
            .AddHttpContextAccessor()
            .AddScoped<IPermissionGuard, PermissionGuard>()
            .AddScoped<IUserContextProvider, UserContextProvider>();

        services
            .AddMappers()
            .AddAll(typeof(ISortingProvider<>));
        
        return services;
    }

    private static IServiceCollection AddMediatR(this IServiceCollection services)
    {
        services.AddMediatrWithValidation(ThisAssembly)
            .AddPipelineBehavior(typeof(PagingValidationBehavior<,>));
        
        return services;
    }

    private static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration config)
    {
        MongoDbClassMaps.Register();
        
        var settings = config.GetSettings<MongoDbSettings>()!;
        var db = new MongoClient(settings.ConnectionUri)
            .GetDatabase(settings.DatabaseName);
        
        services.AddSingleton(db.GetCollection<Workspace>(settings.Collections.Workspaces));
        services.AddSingleton(db.GetCollection<Note>(settings.Collections.Notes));

        services.AddScoped<IWorkspaceRepository, WorkspaceMongoRepository>();
        services.AddScoped<INoteRepository, NoteMongoRepository>();

        return services;
    }

    private static IServiceCollection AddMappers(this IServiceCollection services)
        => services.AddAll(typeof(IMapper<,>));

    private static IServiceCollection AddAll(this IServiceCollection services,
        Type parentType,
        ServiceLifetime lifetime = ServiceLifetime.Scoped)
        => services.Scan(scan
            => scan.FromAssemblies(ThisAssembly)
                .AddClasses(classes => classes.AssignableTo(parentType))
                .AsImplementedInterfaces()
                .WithLifetime(lifetime));
}
