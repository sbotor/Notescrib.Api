using System.Reflection;
using MediatR;
using MediatR.Extensions.FluentValidation.AspNetCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notescrib.Notes.Application.Contracts;
using Notescrib.Notes.Application.Features.Workspaces;
using Notescrib.Notes.Application.Models.Configuration;
using Notescrib.Notes.Application.Services;

namespace Notescrib.Notes.Application;

public static class ServicesExtensions
{
    private static readonly Assembly ThisAssembly = typeof(ServicesExtensions).Assembly;
    
    public static IServiceCollection AddRequiredServices(this IServiceCollection services, IConfiguration config)
    {
        services.AddMediatR(ThisAssembly).AddFluentValidation(new[] { ThisAssembly });

        services.AddMongoDb(config);

        services
            .AddHttpContextAccessor()
            .AddScoped<ISharingGuard, SharingGuard>()
            .AddScoped<IUserContextProvider, UserContextProvider>();

        services.AddMappers();
        
        return services;
    }

    private static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<MongoDbSettings>(config.GetSection(nameof(MongoDbSettings)));

        services.AddSingleton<IMongoCollectionProvider, MongoCollectionProvider>();

        services.AddMongoCollection<Workspace>(x => x.Workspaces);

        return services;
    }

    private static IServiceCollection AddMongoCollection<T>(this IServiceCollection services, Func<CollectionNames, string> nameResolver)
        where T : class
        => services.AddScoped(serviceProvider =>
        {
            var collectionProvider = serviceProvider.GetRequiredService<IMongoCollectionProvider>();
            return collectionProvider.GetCollection<T>(nameResolver.Invoke(collectionProvider.CollectionNames));
        });

    private static IServiceCollection AddMappers(this IServiceCollection services)
    {
        services.Scan(scan
            => scan.FromAssemblies(ThisAssembly)
                .AddClasses(classes => classes.AssignableTo(typeof(IMapper<,>)))
                .AsImplementedInterfaces()
                .WithSingletonLifetime());
        
        return services;
    }
}
