using System.Net;

namespace Notescrib.Api.Core;

public class ApiResponse<TResponse, TError>
{
    public TResponse? Response { get; set; }
    public TError? Error { get; set; }
    public bool IsSuccessful { get; set; }
    public HttpStatusCode? StatusCode { get; set; }
}

public class ApiResponse<TResponse> : ApiResponse<TResponse, string>
{
    public ApiResponse<TCast> CastError<TCast>()
    => new()
    {
        Error = Error,
        IsSuccessful = IsSuccessful,
        StatusCode = StatusCode,
    };

    public static ApiResponse<TResponse> Failure(string? error = null, HttpStatusCode statusCode = HttpStatusCode.BadRequest)
        => new()
        {
            IsSuccessful = false,
            Error = error ?? "An error has occured.",
            StatusCode = statusCode
        };

    public static ApiResponse<TResponse> Success(TResponse response, HttpStatusCode statusCode = HttpStatusCode.OK)
        => new()
        {
            IsSuccessful = true,
            Response = response,
            StatusCode = statusCode
        };

    public static ApiResponse<TResponse> NotFound(string? error = null)
        => new()
        {
            IsSuccessful = false,
            Error = error ?? "The resource was not found.",
            StatusCode = HttpStatusCode.NotFound
        };

    public static ApiResponse<TResponse> Forbidden(string? error = null)
        => new()
        {
            IsSuccessful = false,
            Error = error ?? "Invalid permissions to access this resource.",
            StatusCode = HttpStatusCode.Forbidden
        };
}
