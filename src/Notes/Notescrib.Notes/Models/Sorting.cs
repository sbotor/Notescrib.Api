using System.Globalization;
using Notescrib.Notes.Application.Models.Enums;
using Notescrib.Notes.Application.Models.Exceptions;

namespace Notescrib.Notes.Application.Models;

public readonly struct Sorting
{
    public string OrderBy { get; }
    public SortingDirection Direction { get; }

    private Sorting(string orderBy, SortingDirection direction = SortingDirection.Ascending)
    {
        OrderBy = orderBy;
        Direction = direction;
    }

    public static Sorting Parse<T>(string? orderBy, SortingDirection direction, string defaultOrderBy)
    {
        if (orderBy == null)
        {
            return new(defaultOrderBy, direction);
        }

        var propertyName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(orderBy);
        var property = typeof(T).GetProperty(propertyName)
                       ?? throw new AppException($"Invalid property name '{propertyName}'.");

        return new(property.Name, direction);
    }
}
