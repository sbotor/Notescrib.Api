namespace Notescrib.Api.Core.Contracts;

public interface IPaging
{
    public int PageNumber { get; }
    public int PageSize { get; }
}
