namespace Notescrib.Core.Models;

public class ErrorResponse
{
    public IReadOnlyCollection<ErrorModel>? Errors { get; set; }
    public IReadOnlyCollection<ValidationErrorModel>? ValidationErrors { get; set; }

    public static ErrorResponse UnexpectedError => new()
    {
        Errors = new[] { new ErrorModel(GeneralErrorCodes.UnexpectedError) }
    };
}
