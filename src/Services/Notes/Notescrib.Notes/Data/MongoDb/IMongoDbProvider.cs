using MongoDB.Driver;
using Notescrib.Notes.Features.Folders;
using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Templates;
using Notescrib.Notes.Features.Workspaces;

namespace Notescrib.Notes.Data.MongoDb;

public interface IMongoDbProvider
{
    IMongoCollection<Workspace> Workspaces { get; }
    IMongoCollection<FolderData> Folders { get; }
    IMongoCollection<NoteData> Notes { get; }
    IMongoCollection<NoteTemplate> NoteTemplates { get; }
    bool SupportsTransactions { get; }
    Task<IClientSessionHandle> CreateSessionAsync(CancellationToken cancellationToken = default);
}
