using System.Globalization;
using Notescrib.Api.Core.Exceptions;

namespace Notescrib.Api.Core.Extensions;

public static class StringExtensions
{
    public static string? FindPropertyName<T>(this string str)
    {
        var propName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(str.Trim());
        return typeof(T).GetProperty(propName)?.Name;
    }

    public static string FindPropertyNameOrThrow<T>(this string str)
        => str.FindPropertyName<T>()
            ?? throw new AppException($"Cannot find property for '{str}' in type {typeof(T).Name}.");
}
