namespace Notescrib.Core.Models.Exceptions;

public class RequestValidationException : AppException
{
    private readonly IReadOnlyCollection<ValidationErrorModel> _errors;

    public RequestValidationException(IEnumerable<ValidationErrorModel> errors)
    {
        _errors = errors.ToArray();
    }

    public override ErrorResponse ToResponse()
        => new() { ValidationErrors = _errors };
}
