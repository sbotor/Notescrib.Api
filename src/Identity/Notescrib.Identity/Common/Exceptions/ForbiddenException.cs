namespace Notescrib.Identity.Common.Exceptions;

public class ForbiddenException : AppException
{
    public ForbiddenException(string? message = null) : base(message)
    {
    }
}
