namespace Notescrib.Core.Models.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string? message = null) : base(message)
    {
    }
}

public class NotFoundException<T> : NotFoundException
{
    public NotFoundException(string? id = null)
        : base(FormatMsg(typeof(T).Name, id))
    {
    }

    private static string FormatMsg(string name, string? id)
        => id != null
            ? $"{name} with Id {id} not found."
            : $"{name} not found.";
}
