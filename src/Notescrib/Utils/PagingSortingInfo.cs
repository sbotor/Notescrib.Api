using Notescrib.Contracts;
using Notescrib.Models;

namespace Notescrib.Utils;

public record PagingSortingInfo<TSort>(Paging Paging, Sorting<TSort> Sorting, ISortingProvider<TSort> SortingProvider)
    where TSort : struct, Enum;
