using Notescrib.Notes.Application.Models.Enums;

namespace Notescrib.Notes.Application.Models;

public class SharingInfo
{
    public VisibilityLevel Visibility { get; set; } = VisibilityLevel.Private;
    public ICollection<string> AllowedIds { get; set; } = new List<string>();
}
