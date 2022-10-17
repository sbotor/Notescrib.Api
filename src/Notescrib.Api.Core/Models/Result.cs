using System.Net;

namespace Notescrib.Api.Core.Models;

public class Result
{
    public ErrorModel? Error { get; set; }
    public bool IsSuccessful { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
}

public class Result<TResponse> : Result
{
    private const string NotFoundMsg = "The resource was not found.";
    private const string ForbiddenMsg = "Invalid permissions to access this resource.";

    public TResponse? Response { get; set; }

    public static Result<TResponse> Success(TResponse? response, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new()
        {
            IsSuccessful = true,
            StatusCode = statusCode,
            Response = response
        };

    public static Result<TResponse> Created(TResponse? response)
        => Success(response, HttpStatusCode.Created);

    public static Result<TResponse> Failure(string? message = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new()
        {
            IsSuccessful = false,
            Error = new(message),
            StatusCode = statusCode
        };

    public static Result<TResponse> NotFound(string? message = null)
        => Failure(message ?? NotFoundMsg, HttpStatusCode.NotFound);

    public static Result<TResponse> Forbidden(string? message = null)
        => Failure(message ?? ForbiddenMsg, HttpStatusCode.Forbidden);
}
