using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notescrib.Notes.Models.Configuration;
using Notescrib.Notes.Utils.MongoDb;

namespace Notescrib.Notes;

public static class MongoDbSetupExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration config)
    {
        MongoDbConfig.RegisterClassMaps();

        services.Configure<MongoDbSettings>(config.GetSection(nameof(MongoDbSettings)));

        services.AddSingleton<MongoDbContext>();

        return services;
    }
}
