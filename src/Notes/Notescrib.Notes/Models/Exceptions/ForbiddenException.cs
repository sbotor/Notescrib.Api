namespace Notescrib.Notes.Application.Models.Exceptions;

public class ForbiddenException : AppException
{
    public ForbiddenException(string? message = null) : base(message)
    {
    }
}
