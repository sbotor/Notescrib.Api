using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notescrib.Data.MongoDb;
using Notescrib.Models.Configuration;

namespace Notescrib;

public static class MongoDbSetupExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration config)
    {
        MongoDbConfig.RegisterClassMaps();

        services.Configure<MongoDbSettings>(config.GetSection(nameof(MongoDbSettings)));

        services.AddSingleton<IMongoDbProvider, MongoDbProvider>();

        services.AddScoped<IMongoDbContext, MongoDbContext>();

        return services;
    }
}
