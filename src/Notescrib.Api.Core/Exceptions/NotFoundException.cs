using System.Net;

namespace Notescrib.Api.Core.Exceptions;

public class NotFoundException : AppException
{
    private const HttpStatusCode DefaultStatusCode = HttpStatusCode.NotFound;
    
    public NotFoundException(string? message = null)
        : base(message, null, DefaultStatusCode)
    {
    }
}
