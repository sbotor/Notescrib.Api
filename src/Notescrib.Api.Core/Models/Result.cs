using System.Net;

namespace Notescrib.Api.Core.Models;

public class Result
{
    protected const string NotFoundMsg = "The resource was not found.";
    protected const string ForbiddenMsg = "Invalid permissions to access this resource.";

    public ErrorModel? Error { get; init; }
    public virtual bool IsSuccessful => Error != null;
    public HttpStatusCode? StatusCode { get; init; }

    public static Result Success(HttpStatusCode statusCode = HttpStatusCode.NoContent)
        => new() { StatusCode = statusCode };

    public static Result Failure(string? message = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new() { Error = new(message), StatusCode = statusCode };

    public static Result InternalError(string? message = null)
        => Failure(message, HttpStatusCode.InternalServerError);

    public static Result NotFound(string? message = null)
        => Failure(message ?? NotFoundMsg, HttpStatusCode.NotFound);

    public static Result Forbidden(string? message = null)
        => Failure(message ?? ForbiddenMsg, HttpStatusCode.Forbidden);
}

public class Result<T> : Result
{
    public T? Response { get; }
    public override bool IsSuccessful => base.IsSuccessful && Response != null;

    public Result()
    {
    }

    public Result(Result source)
    {
        Error = source.Error;
        StatusCode = source.StatusCode;
    }

    public Result(Result source, T? response) : this(source)
    {
        Response = response;
    }

    public Result<TOut> CastError<TOut>()
        => new(this);

    public Result CastError()
        => new() { Error = Error, StatusCode = StatusCode };

    public static Result<T> Success(T? response, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new(Success(statusCode), response);

    public static Result<T> Created(T? response)
        => Success(response, HttpStatusCode.Created);

    public static new Result<T> Failure(string? message = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new(Result.Failure(message, statusCode));

    public static new Result<T> InternalError(string? message = null)
        => new(Result.InternalError(message));

    public static new Result<T> NotFound(string? message = null)
        => new(Result.NotFound(message));

    public static new Result<T> Forbidden(string? message = null)
        => new(Result.Forbidden(message));
}
