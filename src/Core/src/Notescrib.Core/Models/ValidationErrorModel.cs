using System.Text.Json;

namespace Notescrib.Core.Models;

public class ValidationErrorModel
{
    public string Message { get; set; } = null!;
    public IReadOnlyDictionary<string, string[]> Errors { get; set; } = null!;

    public string Serialize()
        => JsonSerializer.Serialize(this);
}
