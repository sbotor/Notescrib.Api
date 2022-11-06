using System.Net;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Core.Exceptions;

public class RequestValidationException : AppException
{
    private const string ErrorMsg = "Validation errors occured.";

    public RequestValidationException(IEnumerable<ErrorItem> errorData)
        : base(ErrorMsg, errorData)
    {
    }

    public RequestValidationException(ErrorItem error)
        : base(ErrorMsg, new[] { error })
    {
    }
}
