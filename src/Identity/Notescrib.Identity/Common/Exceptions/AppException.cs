namespace Notescrib.Identity.Common.Exceptions;

public class AppException : Exception
{
    public AppException(string? message = null) : base(message)
    {
    }
}
