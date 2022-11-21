using Notescrib.Notes.Models;

namespace Notescrib.Notes.Contracts;

public interface IShareable
{
    SharingInfo SharingInfo { get; }
    string OwnerId { get; }
}
