using System.Net;

namespace Notescrib.Api.Core.Models;

public class Result
{
    public string? Error { get; set; }
    public bool IsSuccessful { get; set; }
    public HttpStatusCode? StatusCode { get; set; }

    public Result<TCast> CastError<TCast>()
        => new()
        {
            Error = Error,
            IsSuccessful = IsSuccessful,
            StatusCode = StatusCode,
        };

    public static Result Success(HttpStatusCode statusCode = HttpStatusCode.OK)
        => new()
        {
            IsSuccessful = true,
            StatusCode = statusCode
        };

    public static Result Failure(string? error = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new()
        {
            IsSuccessful = false,
            Error = error ?? "An error has occured.",
            StatusCode = statusCode
        };

    public static Result NotFound(string? error = null)
        => new()
        {
            IsSuccessful = false,
            Error = error ?? "The resource was not found.",
            StatusCode = HttpStatusCode.NotFound
        };

    public static Result Forbidden(string? error = null)
        => new()
        {
            IsSuccessful = false,
            Error = error ?? "Invalid permissions to access this resource.",
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

    public static new Result<TResponse> Failure(string? error = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new(Result.Failure(error, statusCode));

    public static new Result<TResponse> NotFound(string? error = null)
        => new(Result.NotFound(error));

    public static new Result<TResponse> Forbidden(string? error = null)
        => new(Result.Forbidden(error));
}
