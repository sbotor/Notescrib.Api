using MongoDB.Driver;

namespace Notescrib.Data.MongoDb;

public class SessionAccessor : IDisposable, IAsyncDisposable
{
    private readonly IMongoDbProvider _provider;

    private bool _disposed;
    
    public IClientSessionHandle? Session { get; private set; }
    public bool SupportsTransactions => _provider.SupportsTransactions;

    public SessionAccessor(IMongoDbProvider provider)
    {
        _provider = provider;
    }
    
    public async ValueTask EnsureTransactionAsync(CancellationToken cancellationToken = default)
    {
        ThrowIfDisposed();
        
        Session ??= await _provider.CreateSessionAsync(cancellationToken);

        if (SupportsTransactions && !Session.IsInTransaction)
        {
            Session.StartTransaction();
        }
    }

    public async ValueTask CommitTransactionAsync()
    {
        if (!SupportsTransactions)
        {
            return;
        }
        
        if (Session?.IsInTransaction != true)
        {
            throw new InvalidOperationException("No current transaction.");
        }

        await Session.CommitTransactionAsync();
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(SessionAccessor));
        }
    }
    
    protected virtual void Dispose(bool disposing)
    {
        if (_disposed || !disposing)
        {
            return;
        }
        
        Session?.Dispose();
        _disposed = true;
    }
    
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual async ValueTask DisposeAsyncCore()
    {
        if (Session != null)
        {
            if (SupportsTransactions && Session.IsInTransaction)
            {
                await Session.AbortTransactionAsync();
            }
            
            Session.Dispose();
            Session = null;
        }
    }
    
    public async ValueTask DisposeAsync()
    {
        await DisposeAsyncCore().ConfigureAwait(false);
        
        Dispose(false);
        GC.SuppressFinalize(this);
    }
}
