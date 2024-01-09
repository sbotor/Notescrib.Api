namespace Notescrib.Core.Models.Exceptions;

public class DuplicationException : AppException
{
    public DuplicationException(string code, string? message = null)
        : base(code, message)
    {
    }
}
