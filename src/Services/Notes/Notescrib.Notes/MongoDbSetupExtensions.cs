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

        var section = config.GetSection(nameof(MongoDbSettings));
        services.Configure<MongoDbCollectionNames>(section.GetSection(nameof(MongoDbCollectionNames)));
        
        var settings = section.Get<MongoDbSettings>()!;
        var db = new MongoClient(settings.ConnectionUri)
            .GetDatabase(settings.DatabaseName);
        
        services.AddSingleton(db.GetCollection<Workspace>(settings.Collections.Workspaces));
        services.AddScoped<IWorkspaceRepository, WorkspaceMongoRepository>();
        
        services.AddSingleton(db.GetCollection<FolderBase>(settings.Collections.Folders));
        services.AddScoped<IFolderRepository, FolderMongoRepository>();
        
        services.AddSingleton(db.GetCollection<Note>(settings.Collections.Notes));
        services.AddScoped<INoteRepository, NoteMongoRepository>();

        return services;
    }
}
