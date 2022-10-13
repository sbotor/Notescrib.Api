using System.Text.Json;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Core.Exceptions;

public class RequestValidationException : AppException
{
    public IEnumerable<ValidationError> Errors { get; }

    public RequestValidationException(IEnumerable<ValidationError> errors)
    {
        Errors = errors;
    }

    public RequestValidationException(string key, params string[] messages)
    {
        Errors = new List<ValidationError>()
        {
            new ValidationError(key, messages)
        };
    }

    public RequestValidationException(string? message = null)
        : this("Error", string.IsNullOrEmpty(message) ? Array.Empty<string>() : new[] { message })
    {
    }

    public override string SerializeErrors()
        => JsonSerializer.Serialize(Errors);
}
