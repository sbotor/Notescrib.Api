using Notescrib.Notes.Models.Enums;

namespace Notescrib.Notes.Models;

public class SharingInfo
{
    public VisibilityLevel Visibility { get; set; } = VisibilityLevel.Private;
}
