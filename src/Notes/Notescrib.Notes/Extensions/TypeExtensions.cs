using System.Globalization;
using System.Reflection;

namespace Notescrib.Notes.Extensions;

public static class TypeExtensions
{
    public static PropertyInfo? FindProperty(this Type type, string name)
    {
        var propertyName = CultureInfo.InvariantCulture.TextInfo.ToTitleCase(name);
        return type.GetProperty(propertyName);
    }
}
