using Notescrib.Notes.Models;

namespace Notescrib.Notes.Utils;

public static class PagingHelper
{
    public static int CalculateSkipCount(int page, int pageSize)
        => (page - 1) * pageSize;

    public static int CalculateSkipCount(Paging paging)
        => CalculateSkipCount(paging.Page, paging.PageSize);

    public static int CalculatePageCount(int totalCount, int pageSize)
        => (totalCount + pageSize - 1) / pageSize;
}

