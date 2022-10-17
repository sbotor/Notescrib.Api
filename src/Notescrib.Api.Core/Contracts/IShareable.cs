using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Core.Contracts;

public interface IShareable
{
    public SharingDetails SharingDetails { get; set; }
}
