using System.Net;
using System.Text.Json;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Core.Exceptions;

public class AppException : Exception
{
    public IEnumerable<ErrorItem>? Errors { get; protected set; }
    public HttpStatusCode StatusCode { get; protected set; }

    public AppException(string? message = null, IEnumerable<ErrorItem>? errors = null, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : base(message ?? ErrorModel.DefaultMessage)
    {
        Errors = errors;
        StatusCode = statusCode;
    }

    public AppException(IEnumerable<ErrorItem>? errors = null, HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
    {
        Errors = errors;
        StatusCode = statusCode;
    }

    public AppException(HttpStatusCode statusCode = HttpStatusCode.InternalServerError)
        : this(null, statusCode)
    {
    }

    public Result<TResponse> ToResult<TResponse>()
        => Result<TResponse>.Failure(Serialize(), StatusCode);

    public Result ToResult()
        => Result.Failure(Serialize(), StatusCode);

    public ErrorModel ToErrorModel()
        => new(Message, Errors);

    public string Serialize()
        => JsonSerializer.Serialize(ToErrorModel());
}
