namespace Notescrib.Core.Models.Exceptions;

public class AppException : Exception
{
    private readonly IReadOnlyCollection<ErrorModel> _errors;

    public AppException(string code, string? message = null) : base(message)
    {
        _errors = new ErrorModel[] { new(code, message) };
    }

    public AppException(IEnumerable<ErrorModel> errors)
    {
        _errors = errors.ToArray();
    }

    protected AppException()
    {
        _errors = Array.Empty<ErrorModel>();
    }

    public virtual ErrorResponse ToResponse()
        => new() { Errors = _errors };
}
