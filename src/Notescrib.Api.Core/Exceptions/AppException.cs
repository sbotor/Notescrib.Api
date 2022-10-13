using System.Text.Json;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Core.Exceptions;

public class AppException : Exception
{
    public AppException(string? message = null) : base(message)
    {
    }

    public Result<TResponse> ToResult<TResponse>()
        => Result<TResponse>.Failure(SerializeErrors());

    public Result ToResult()
        => Result.Failure(SerializeErrors());

    public virtual string SerializeErrors()
        => JsonSerializer.Serialize(Message);
}
