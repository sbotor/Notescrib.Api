namespace Notescrib.Notes.Utils.MongoDb.Models;

public class PagedResultModel<T>
{
    public IReadOnlyCollection<int> Counts { get; set; } = null!;
    public IReadOnlyCollection<T> Data { get; set; } = null!;
}
