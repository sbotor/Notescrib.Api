using Notescrib.Features.Folders.Repositories;
using Notescrib.Features.Notes.Repositories;
using Notescrib.Features.Templates.Repositories;
using Notescrib.Features.Workspaces.Repositories;

namespace Notescrib.Data.MongoDb;

public interface IMongoDbContext : IDisposable, IAsyncDisposable
{
    IWorkspaceRepository Workspaces { get; }
    IFolderRepository Folders { get; }
    INoteRepository Notes { get; }
    INoteTemplateRepository NoteTemplates { get; }
    ValueTask EnsureTransactionAsync(CancellationToken cancellationToken = default);
    ValueTask CommitTransactionAsync();
}
