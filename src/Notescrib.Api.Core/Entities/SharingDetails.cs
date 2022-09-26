using Notescrib.Api.Core.Enums;

namespace Notescrib.Api.Core.Entities;

public class SharingDetails
{
    public Visibility Visibility { get; set; }
    public ICollection<string> AllowedUserIds { get; set; } = new List<string>();
}
