using Microsoft.Extensions.Options;
using MongoDB.Driver;
using MongoDB.Driver.Core.Clusters;
using Notescrib.Features.Folders;
using Notescrib.Features.Notes;
using Notescrib.Features.Templates;
using Notescrib.Features.Workspaces;
using Notescrib.Models.Configuration;

namespace Notescrib.Data.MongoDb;

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
