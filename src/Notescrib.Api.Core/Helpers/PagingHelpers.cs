namespace Notescrib.Api.Core.Helpers;

public static class PagingHelpers
{
    public static int CalculateSkipCount(int pageNumber, int pageSize)
        => (pageNumber - 1) * pageSize;

    public static int CalculatePageCount(int totalCount, int pageSize)
        => (totalCount + pageSize - 1) / pageSize;
}
