namespace Notescrib.Api.Core.Models;

public class ValidationError
{
    private static readonly string[] DefaultMessages = new[] { "Invalid request." };

    public string Key { get; set; } = string.Empty;
    public IReadOnlyCollection<string> Messages { get; set; } = Array.Empty<string>();

    public ValidationError()
    {
    }

    public ValidationError(string key, params string[] messages)
    {
        Key = key;
        Messages = messages.Any() ? messages : DefaultMessages;
    }
}
