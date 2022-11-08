using Notescrib.Notes.Core.Entities;

namespace Notescrib.Notes.Application.Notes.Models;

public class NoteOverview
{
    public string Id { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public IReadOnlyCollection<string> Labels { get; set; } = new List<string>();
    public SharingInfo SharingInfo { get; set; } = new();
}
