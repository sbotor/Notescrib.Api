namespace Notescrib.Api.Core.Models;

public class ErrorItem
{
    internal static readonly string DefaultKey = "Error";
    internal static readonly string DefaultMessage = "An unknown error has occured.";
    internal static readonly string[] DefaultMessages = { DefaultMessage };

    public string Key { get; set; } = DefaultKey;
    public IReadOnlyCollection<string> Messages { get; set; } = DefaultMessages;

    public ErrorItem()
    {
    }

    public ErrorItem(string key, params string[] messages)
    {
        Key = key;
        Messages = messages.Any() ? messages : DefaultMessages;
    }
}
