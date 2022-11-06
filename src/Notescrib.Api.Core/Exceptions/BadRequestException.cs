using System.Net;

namespace Notescrib.Api.Core.Exceptions;

public class BadRequestException : AppException
{
    private const HttpStatusCode DefaultStatusCode = HttpStatusCode.BadRequest;
    
    public BadRequestException(string message) : base(message, null, DefaultStatusCode)
    {
    }

    public BadRequestException() : base(DefaultStatusCode)
    {
    }
}
