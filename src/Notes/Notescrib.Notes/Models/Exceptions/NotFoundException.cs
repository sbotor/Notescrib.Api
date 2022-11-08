namespace Notescrib.Notes.Application.Models.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string? message = null) : base(message)
    {
    }
}
