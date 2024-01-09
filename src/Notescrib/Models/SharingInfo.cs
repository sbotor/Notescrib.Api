using Notescrib.Models.Enums;

namespace Notescrib.Models;

public class SharingInfo
{
    public VisibilityLevel Visibility { get; set; } = VisibilityLevel.Private;
}
