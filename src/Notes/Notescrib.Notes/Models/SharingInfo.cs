using Notescrib.Notes.Models.Enums;

namespace Notescrib.Notes.Models;

public class SharingInfo
{
    public VisibilityLevel Visibility { get; set; } = VisibilityLevel.Private;
    public ICollection<string> AllowedIds { get; set; } = new List<string>();
}
