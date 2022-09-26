using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notescrib.Api.Application.Repositories;
using Notescrib.Api.Infrastructure.Repositories;
using Notescrib.Api.Infrastructure.Services;

namespace Notescrib.Api.Infrastructure;

public static class InfrastructureExtensions
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration config)
    {
        services.Configure<MongoDbSettings>(config.GetSection(nameof(MongoDbSettings)));

        services.AddSingleton<IMongoCollectionProvider, MongoCollectionProvider>();
        services.AddScoped(typeof(IMongoPersistenceProvider<>), typeof(MongoPersistenceProvider<>));

        services.AddScoped<IWorkspaceRepository, WorkspaceRepository>();

        return services;
    }
}
