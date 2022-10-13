namespace Notescrib.Api.Application.Notes.Contracts;

public class NoteOverviewResponse
{
    public string Name { get; set; } = string.Empty;
    public IReadOnlyCollection<string> Labels { get; set; } = new List<string>();
}
