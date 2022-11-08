namespace Notescrib.Notes.Core.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string? message = null) : base(message)
    {
    }

    public NotFoundException(string entityName, string entityId)
        : base($"Entity '{entityName}' with Id '{entityId}' not found.")
    {
    }

    public static NotFoundException FromType<T>(string id)
        => new(typeof(T).Name, id);
}
