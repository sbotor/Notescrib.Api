using System.Globalization;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Enums;
using Notescrib.Api.Core.Exceptions;

namespace Notescrib.Api.Core.Models;

public class Sorting : ISorting
{
    public string OrderBy { get; set; }
    public SortingDirection Direction { get; set; } = SortingDirection.Ascending;

    public Sorting(string orderBy, SortingDirection direction = SortingDirection.Ascending)
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
        var property = typeof(T).GetProperty(propertyName) ?? throw GetException(orderBy);

        return new(property.Name, direction);
    }

    public static Sorting Parse<T>(ISortingNullable sorting, string defaultOrderBy)
        => Parse<T>(sorting.OrderBy, sorting.Direction, defaultOrderBy);

    private static RequestValidationException GetException(string propertyName)
    {
        var errorItem = new ErrorItem(nameof(ISorting.OrderBy), $"Invalid property name '{propertyName}'.");
        return new(errorItem);
    }
}
