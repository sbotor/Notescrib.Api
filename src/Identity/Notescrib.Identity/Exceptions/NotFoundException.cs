namespace Notescrib.Identity.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string? message = null) : base(message)
    {
    }
}
