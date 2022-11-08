using MediatR;
using Notescrib.Notes.Core.Contracts;

namespace Notescrib.Notes.Application.Common.Contracts;

internal interface IPagingRequest<out TResponse> : IRequest<TResponse>
{
    public IPaging Paging { get; init; }
}
