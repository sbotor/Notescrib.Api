using System.Net;

namespace Notescrib.Api.Core.Models;

public class Result
{
    protected const string NotFoundMsg = "The resource was not found.";
    protected const string ForbiddenMsg = "Invalid permissions to access this resource.";

    public ErrorModel? Error { get; set; }
    public bool IsSuccessful { get; set; }
    public HttpStatusCode? StatusCode { get; set; }

    public static Result Success(HttpStatusCode statusCode = HttpStatusCode.NoContent)
        => new()
        {
            IsSuccessful = true,
            StatusCode = statusCode
        };

    public static Result Failure(string? message = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new()
        {
            IsSuccessful = false,
            Error = new(message),
            StatusCode = statusCode
        };

    public static Result InternalError(string? message = null)
        => Failure(message, HttpStatusCode.InternalServerError);

    public static Result NotFound(string? message = null)
        => Failure(message ?? NotFoundMsg, HttpStatusCode.NotFound);

    public static Result Forbidden(string? message = null)
        => Failure(message ?? ForbiddenMsg, HttpStatusCode.Forbidden);
}

public class Result<T> : Result
{
    public T? Response { get; set; }

    public Result()
    {
    }

    public Result(Result source)
    {
        IsSuccessful = source.IsSuccessful;
        Error = source.Error;
        StatusCode = source.StatusCode;
    }

    public Result(Result source, T? response) : this(source)
    {
        Response = response;
    }

    public Result<TOut> Map<TOut>()
        => new(this);

    public static Result<T> Success(T? response, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(Success(statusCode), response);

    public static Result<T> Created(T? response)
        => Success(response, HttpStatusCode.Created);

    public static new Result<T> Failure(string? message = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new(Failure(message, statusCode));

    public static new Result<T> InternalError(string? message = null)
        => Failure(message, HttpStatusCode.InternalServerError);

    public static new Result<T> NotFound(string? message = null)
        => new(Failure(message, HttpStatusCode.NotFound));

    public static new Result<T> Forbidden(string? message = null)
        => new(Failure(message, HttpStatusCode.Forbidden));
}
