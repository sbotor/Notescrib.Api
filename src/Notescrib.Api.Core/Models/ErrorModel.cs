namespace Notescrib.Api.Core.Models;

public class ErrorModel
{
    internal const string DefaultMessage = "An error has occured.";

    public IReadOnlyCollection<ErrorItem>? ErrorData { get; set; }

    public string ErrorMessage { get; } = DefaultMessage;

    public ErrorModel(string? message = null, IEnumerable<ErrorItem>? errors = null)
    {
        ErrorMessage = message ?? DefaultMessage;
        ErrorData = errors?.ToList();
    }

    public ErrorModel(Exception exception)
    {
        ErrorMessage = exception.Message;
    }
}
