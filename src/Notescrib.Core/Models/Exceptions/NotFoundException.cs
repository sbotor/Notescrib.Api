namespace Notescrib.Core.Models.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string? code = null, string? message = null)
        : base(code ?? GeneralErrorCodes.NotFound, message)
    {
    }
}
