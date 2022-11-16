using Notescrib.Notes.Contracts;
using Notescrib.Notes.Models;

namespace Notescrib.Notes.Utils;

public record PagingSortingInfo<TSort>(Paging Paging, Sorting<TSort> Sorting, ISortingProvider<TSort> SortingProvider)
    where TSort : struct, Enum;
