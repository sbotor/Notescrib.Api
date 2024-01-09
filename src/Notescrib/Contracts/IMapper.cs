namespace Notescrib.Contracts;

public interface IMapper<in TSource, out TDest>
{
    TDest Map(TSource item);
}
