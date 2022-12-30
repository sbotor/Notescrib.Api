using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Templates.Repositories;
using Notescrib.Notes.Features.Workspaces.Repositories;

namespace Notescrib.Notes.Data.MongoDb;

public interface IMongoDbContext : IDisposable, IAsyncDisposable
{
    IWorkspaceRepository Workspaces { get; }
    IFolderRepository Folders { get; }
    INoteRepository Notes { get; }
    INoteTemplateRepository NoteTemplates { get; }
    ValueTask EnsureTransactionAsync(CancellationToken cancellationToken = default);
    ValueTask CommitTransactionAsync();
}
