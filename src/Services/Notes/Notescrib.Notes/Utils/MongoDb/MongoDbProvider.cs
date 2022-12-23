using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using Notescrib.Notes.Features.Folders;
using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Templates;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Models.Configuration;

namespace Notescrib.Notes.Utils.MongoDb;

public class MongoDbProvider : IMongoDbProvider
{
    private readonly IMongoDatabase _db;
    
    public IMongoCollection<Workspace> Workspaces { get; }
    public IMongoCollection<FolderData> Folders { get; }
    public IMongoCollection<NoteData> Notes { get; }
    public IMongoCollection<NoteTemplate> NoteTemplates { get; }

    public bool SupportsTransactions
        => _db.Client.Cluster.Description.Type is ClusterType.ReplicaSet or ClusterType.Sharded;
    
    public MongoDbProvider(IOptions<MongoDbSettings> options)
    {
        var settings = options.Value;

        var client = new MongoClient(settings.ConnectionUri);

        _db = client
            .GetDatabase(settings.DatabaseName);

        Workspaces = _db.GetCollection<Workspace>(settings.Collections.Workspaces);
        Folders = _db.GetCollection<FolderData>(settings.Collections.Folders);
        Notes = _db.GetCollection<NoteData>(settings.Collections.Notes);
        NoteTemplates = _db.GetCollection<NoteTemplate>(settings.Collections.NoteTemplates);
    }

    public Task<IClientSessionHandle> CreateSessionAsync(CancellationToken cancellationToken = default)
        => _db.Client.StartSessionAsync(cancellationToken: cancellationToken);
}
