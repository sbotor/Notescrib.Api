namespace Notescrib.Api.Application.Contracts.Workspace;

public class NoteDetails
{
    public string Name { get; set; } = string.Empty;
    public IReadOnlyCollection<string> Labels { get; set; } = new List<string>();
}
