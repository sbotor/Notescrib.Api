using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Templates.Repositories;
using Notescrib.Notes.Features.Workspaces.Repositories;
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

        services.AddSingleton<IFolderRepository, MongoFolderRepository>();
        services.AddSingleton<INoteContentRepository, MongoNoteContentRepository>();
        services.AddSingleton<IWorkspaceRepository, MongoWorkspaceRepository>();
        services.AddSingleton<INoteTemplateRepository, MongoNoteTemplateRepository>();

        return services;
    }
}
