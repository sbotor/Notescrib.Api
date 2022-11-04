using Notescrib.Api.Core.Enums;

namespace Notescrib.Api.Core.Entities;

public class SharingInfo
{
    public VisibilityLevel Visibility { get; set; } = VisibilityLevel.Private;
    public ICollection<string> AllowedUserIds { get; set; } = new List<string>();
}
