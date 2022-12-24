using Notescrib.Core.Services;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Templates.Repositories;
using Notescrib.Notes.Features.Workspaces.Repositories;

namespace Notescrib.Notes.Utils.MongoDb;

public class MongoDbContext : IDisposable, IAsyncDisposable
{
    private readonly IMongoDbProvider _provider;
    private readonly IUserContextProvider _userContextProvider;

    private bool _disposed;
    private readonly SessionAccessor _sessionAccessor;

    public MongoDbContext(IMongoDbProvider provider, IUserContextProvider userContextProvider)
    {
        _provider = provider;
        _userContextProvider = userContextProvider;
        _sessionAccessor = new(_provider);
    }

    private MongoWorkspaceRepository? _workspaces;
    public IWorkspaceRepository Workspaces => _workspaces ??= new(_provider, _sessionAccessor, _userContextProvider);

    private MongoFolderRepository? _folders;
    public IFolderRepository Folders => _folders ??= new(_provider, _sessionAccessor, _userContextProvider);
    
    private MongoNoteRepository? _notes;
    public INoteRepository Notes => _notes ??= new(_provider, _sessionAccessor, _userContextProvider);

    private MongoNoteTemplateRepository? _noteTemplates;
    public INoteTemplateRepository NoteTemplates => _noteTemplates ??= new(_provider, _sessionAccessor, _userContextProvider);

    public async ValueTask EnsureTransactionAsync(CancellationToken cancellationToken = default)
        => await _sessionAccessor.EnsureTransactionAsync(cancellationToken);

    public async ValueTask CommitTransactionAsync()
        => await _sessionAccessor.CommitTransactionAsync();

    private void Dispose(bool disposing)
    {
        if (_disposed || !disposing)
        {
            return;
        }
        
        _sessionAccessor.Dispose();
        _disposed = true;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    private async ValueTask DisposeAsyncCore()
        => await _sessionAccessor.DisposeAsync();

    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        
        Dispose(false);
        GC.SuppressFinalize(this);
    }
}
