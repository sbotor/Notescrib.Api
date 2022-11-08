using System.Globalization;
using Notescrib.Notes.Models.Enums;
using Notescrib.Notes.Models.Exceptions;

namespace Notescrib.Notes.Models;

public struct Sorting
{
    internal bool IsSafe { get; }
    
    public string OrderBy { get; internal set; }
    public SortingDirection Direction { get; }

    public Sorting(string? orderBy, SortingDirection direction, string defaultSafeOrderBy)
    {
        if (string.IsNullOrEmpty(orderBy))
        {
            OrderBy = defaultSafeOrderBy;
            IsSafe = true;
        }
        else
        {
            OrderBy = orderBy;
            IsSafe = false;
        }
        
        Direction = direction;
    }
}
