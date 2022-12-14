using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MongoDB.Driver;
using Notescrib.Core.Extensions;
using Notescrib.Notes.Features.Folders;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Models.Configuration;
using Notescrib.Notes.Utils.MongoDb;

namespace Notescrib.Notes;

public static class MongoDbSetupExtensions
{
    public static IServiceCollection AddMongoDb(this IServiceCollection services, IConfiguration config)
    {
        MongoDbClassMaps.Register();

        services.Configure<MongoDbSettings>(config.GetSection(nameof(MongoDbSettings)));

        services.AddSingleton<MongoDbContext>();

        return services;
    }
}
