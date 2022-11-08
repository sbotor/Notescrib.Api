using System.Globalization;
using Notescrib.Notes.Core.Contracts;
using Notescrib.Notes.Core.Exceptions;
using Notescrib.Notes.Core.Models.Enums;

namespace Notescrib.Notes.Core.Models;

public class Sorting : ISorting
{
    public string OrderBy { get; set; }
    public SortingDirection Direction { get; set; }

    private Sorting(string orderBy, SortingDirection direction = SortingDirection.Ascending)
    {
        OrderBy = orderBy;
        Direction = direction;
    }

    public static Sorting Parse<T>(string? orderBy, SortingDirection direction, string? defaultOrderBy = null)
    {
        if (orderBy == null)
        {
            if (string.IsNullOrEmpty(defaultOrderBy))
            {
                throw new AppException();
            }
            
            return new(defaultOrderBy, direction);
        }

        var propertyName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(orderBy);
        var property = typeof(T).GetProperty(propertyName)
                       ?? throw new AppException($"Invalid sorting property: '{orderBy}'.");

        return new(property.Name, direction);
    }
}
