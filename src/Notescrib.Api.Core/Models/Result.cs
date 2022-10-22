using System.Net;

namespace Notescrib.Api.Core.Models;

public class Result
{
    public ErrorModel? Error { get; set; }
    public bool IsSuccessful { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
}

public class Result<T> : Result
{
    private const string NotFoundMsg = "The resource was not found.";
    private const string ForbiddenMsg = "Invalid permissions to access this resource.";

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

    public Result(Result source, T response) : this(source)
    {
        Response = response;
    }

    public Result<TOut> Map<TOut>()
        => new(this);

    public static Result<T> Success(T? response, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new()
        {
            IsSuccessful = true,
            StatusCode = statusCode,
            Response = response
        };

    public static Result<T> Created(T? response)
        => Success(response, HttpStatusCode.Created);

    public static Result<T> Failure(string? message = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new()
        {
            IsSuccessful = false,
            Error = new(message),
            StatusCode = statusCode
        };

    public static Result<T> NotFound(string? message = null)
        => Failure(message ?? NotFoundMsg, HttpStatusCode.NotFound);

    public static Result<T> Forbidden(string? message = null)
        => Failure(message ?? ForbiddenMsg, HttpStatusCode.Forbidden);
}
