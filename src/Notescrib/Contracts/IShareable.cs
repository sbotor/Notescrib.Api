using Notescrib.Models;

namespace Notescrib.Contracts;

public interface IShareable
{
    SharingInfo SharingInfo { get; }
    string OwnerId { get; }
}
