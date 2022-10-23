using System.Net;

namespace Notescrib.Api.Core.Exceptions;

public class NotFoundException : AppException
{
    public NotFoundException(string? message = null)
        : base(message, null, HttpStatusCode.NotFound)
    {
    }
}
