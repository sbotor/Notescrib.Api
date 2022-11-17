namespace Notescrib.Core.Models.Exceptions;

public class RequestValidationException : AppException
{
    public IReadOnlyDictionary<string, string[]> Errors { get; }

    public RequestValidationException(IReadOnlyDictionary<string, string[]> errors)
        : base("Validation errors occured.")
    {
        Errors = errors;
    }

    public ValidationErrorModel ToErrorModel()
        => new() { Message = Message, Errors = Errors };
}
