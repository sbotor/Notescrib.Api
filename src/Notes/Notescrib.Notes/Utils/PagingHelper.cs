using Notescrib.Notes.Application.Models;

namespace Notescrib.Notes.Application.Utils;

public static class PagingHelper
{
    public static int CalculateSkipCount(int pageNumber, int pageSize)
        => (pageNumber - 1) * pageSize;

    public static int CalculateSkipCount(Paging paging)
        => CalculateSkipCount(paging.PageNumber, paging.PageSize);

    public static int CalculatePageCount(int totalCount, int pageSize)
        => (totalCount + pageSize - 1) / pageSize;
}

