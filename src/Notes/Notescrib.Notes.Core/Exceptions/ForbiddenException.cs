namespace Notescrib.Notes.Core.Exceptions;

public class ForbiddenException : AppException
{
    public ForbiddenException(string? message = null) : base(message)
    {
    }
}
