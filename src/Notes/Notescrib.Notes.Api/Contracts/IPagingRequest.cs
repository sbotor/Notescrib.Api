namespace Notescrib.Notes.Api.Contracts;

public interface IPagingRequest
{
    int PageNumber { get; set; }
    int PageSize { get; set; }
}
