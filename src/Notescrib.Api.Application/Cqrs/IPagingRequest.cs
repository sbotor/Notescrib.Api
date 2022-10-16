using Notescrib.Api.Core.Contracts;

namespace Notescrib.Api.Application.Cqrs;

internal interface IPagingRequest
{
    public IPaging Paging { get; init; }
}
