using System.Net;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Core.Exceptions;

public class RequestValidationException : AppException
{
    private const string ErrorMsg = "Validation errors occured.";
    private const HttpStatusCode DefaultStatusCode = HttpStatusCode.BadRequest;


    public RequestValidationException(IEnumerable<ErrorItem> errors)
        : base(ErrorMsg, errors, DefaultStatusCode)
    {
    }

    public RequestValidationException(ErrorItem error)
        : base(ErrorMsg, new[] { error }, DefaultStatusCode)
    {
    }
}
