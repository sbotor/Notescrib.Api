using MongoDB.Driver;
using Notescrib.Features.Folders;
using Notescrib.Features.Notes;
using Notescrib.Features.Templates;
using Notescrib.Features.Workspaces;

namespace Notescrib.Data.MongoDb;

public interface IMongoDbProvider
{
    IMongoCollection<Workspace> Workspaces { get; }
    IMongoCollection<FolderData> Folders { get; }
    IMongoCollection<NoteData> Notes { get; }
    IMongoCollection<NoteTemplate> NoteTemplates { get; }
    bool SupportsTransactions { get; }
    Task<IClientSessionHandle> CreateSessionAsync(CancellationToken cancellationToken = default);
}
