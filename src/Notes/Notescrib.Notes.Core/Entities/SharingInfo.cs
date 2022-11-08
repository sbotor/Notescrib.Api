using Notescrib.Notes.Core.Models.Enums;

namespace Notescrib.Notes.Core.Entities;

public class SharingInfo
{
    public VisibilityLevel Visibility { get; set; } = VisibilityLevel.Private;
    public ICollection<string> AllowedUserIds { get; set; } = new List<string>();
}
