using Notescrib.Notes.Core.Entities;

namespace Notescrib.Notes.Core.Contracts;

public interface IShareable : IOwnable
{
    public SharingInfo SharingInfo { get; set; }
}
