using System.Net;

namespace Notescrib.Api.Core.Exceptions;

public class BadRequestException : AppException
{
    public BadRequestException(string message) : base(message, null, HttpStatusCode.BadRequest)
    {
    }

    public BadRequestException() : base(HttpStatusCode.BadRequest)
    {
    }
}
