using Notescrib.Models;

namespace Notescrib.Utils;

public static class PagingHelper
{
    public static int CalculatePageCount(int totalCount, int pageSize)
        => (totalCount + pageSize - 1) / pageSize;
}

