namespace Notescrib.Api.Core.Contracts;

public interface IPaging
{
    public int PageNumber { get; set; }
    public int PageSize { get; set; }
}
