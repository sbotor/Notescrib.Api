using System.Net;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Core.Exceptions;

public class RequestValidationException : AppException
{
    private const string ErrorMsg = "Validation errors occured.";

    public RequestValidationException(IEnumerable<ErrorItem> errors)
        : base(ErrorMsg, errors, HttpStatusCode.BadRequest)
    {
    }
}
