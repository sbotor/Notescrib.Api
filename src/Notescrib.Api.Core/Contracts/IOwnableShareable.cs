using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Core.Contracts;

public interface IShareable : IOwnable
{
    public SharingInfo SharingInfo { get; set; }
}
