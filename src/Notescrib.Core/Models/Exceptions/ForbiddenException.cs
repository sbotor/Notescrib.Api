namespace Notescrib.Core.Models.Exceptions;

public class ForbiddenException : AppException
{
    public ForbiddenException(string? code = null , string? message = null)
        : base(code ?? GeneralErrorCodes.Forbidden, message)
    {
    }
}
