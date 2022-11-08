namespace Notescrib.Notes.Application.Contracts;

public interface IMapper<in TSource, out TDest>
{
    TDest Map(TSource item);
}
