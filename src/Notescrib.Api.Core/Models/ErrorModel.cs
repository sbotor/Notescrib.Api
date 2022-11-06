namespace Notescrib.Api.Core.Models;

public class ErrorModel
{
    internal const string DefaultMessage = "An error has occured.";

    public IReadOnlyCollection<ErrorItem> ErrorData { get; }
    public string ErrorMessage { get; }

    public ErrorModel(string? message = null, IEnumerable<ErrorItem>? errors = null)
    {
        ErrorMessage = message ?? DefaultMessage;
        ErrorData = errors != null ? errors.ToList() : Array.Empty<ErrorItem>();
    }

    public ErrorModel(Exception exception)
    {
        ErrorMessage = exception.Message;
        ErrorData = Array.Empty<ErrorItem>();
    }
}
