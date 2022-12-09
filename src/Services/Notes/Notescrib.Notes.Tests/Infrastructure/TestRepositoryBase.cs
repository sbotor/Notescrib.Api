using Notescrib.Notes.Models;
using Notescrib.Notes.Tests.Infrastructure.Extensions;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Tests.Infrastructure;

public class TestRepositoryBase<T>
{
    public List<T> Items { get; set; } = new();

    protected Task Add(T item, Action<T> idGenerator)
    {
        idGenerator.Invoke(item);
        Items.Add(item);
        
        return Task.CompletedTask;
    }

    protected Task Update(T item, Func<T, bool> predicate)
    {
        var found = Items.Single(predicate);
        Items.Remove(found);
        Items.Add(item);
        
        return Task.CompletedTask;
    }
    
    protected Task<bool> Exists(Func<T, bool> predicate)
        => Task.FromResult(Items.Any(predicate));
    
    protected Task<T?> GetSingleOrDefault(Func<T, bool> predicate)
        => Task.FromResult(Items.SingleOrDefault(predicate));
    
    protected Task<IReadOnlyCollection<T>> Get(Func<T, bool> predicate)
        => Task.FromResult<IReadOnlyCollection<T>>(Items.Where(predicate).ToArray());

    protected Task Delete(Func<T, bool> predicate)
    {
        Items = Items.Except(Items.Where(predicate)).ToList();
        return Task.CompletedTask;
    }

    protected Task<long> Count(Func<T, bool> predicate)
        => Task.FromResult(Items.LongCount(predicate));
}

public class TestRepositoryBase<T, TSort> : TestRepositoryBase<T>
    where TSort : struct, Enum
{
    protected Task<PagedList<T>> GetPaged(
        Func<T, bool> filter,
        PagingSortingInfo<TSort> info)
        => Task.FromResult(Items.Where(filter).ToPagedList(info));
}
