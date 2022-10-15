using System.Net;

namespace Notescrib.Api.Core.Models;

public class Result
{
    public ErrorModel? Error { get; set; }
    public bool IsSuccessful { get; set; }
    public HttpStatusCode? StatusCode { get; set; }

    public static Result Success(HttpStatusCode statusCode = HttpStatusCode.OK)
        => new()
        {
            IsSuccessful = true,
            StatusCode = statusCode
        };

    public static Result Created()
        => new()
        {
            IsSuccessful = true,
            StatusCode = HttpStatusCode.Created
        };

    public static Result Failure(string? message = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new()
        {
            IsSuccessful = false,
            Error = new(message),
            StatusCode = statusCode
        };

    public static Result NotFound(string? message = null)
        => new()
        {
            IsSuccessful = false,
            Error = new(message ?? "The resource was not found."),
            StatusCode = HttpStatusCode.NotFound
        };

    public static Result Forbidden(string? error = null)
        => new()
        {
            IsSuccessful = false,
            Error = new(error ?? "Invalid permissions to access this resource."),
            StatusCode = HttpStatusCode.Forbidden
        };
}

public class Result<TResponse> : Result
{
    public TResponse? Response { get; set; }

    public Result()
    {
    }

    public Result(Result result)
    {
        IsSuccessful = result.IsSuccessful;
        Error = result.Error;
        StatusCode = result.StatusCode;
    }

    public Result(Result result, TResponse? response) : this(result)
    {
        Response = response;
    }

    public static Result<TResponse> Success(TResponse response, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(Success(statusCode), response);

    public static Result<TResponse> Created(TResponse response)
        => new(Created(), response);

    public static new Result<TResponse> Failure(string? message = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new(Result.Failure(message, statusCode));

    public static new Result<TResponse> NotFound(string? message = null)
        => new(Result.NotFound(message));

    public static new Result<TResponse> Forbidden(string? message = null)
        => new(Result.Forbidden(message));
}
