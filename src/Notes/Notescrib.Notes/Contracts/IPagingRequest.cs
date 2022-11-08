using MediatR;
using Notescrib.Notes.Application.Models;

namespace Notescrib.Notes.Application.Contracts;

internal interface IPagingRequest<out TResponse> : IRequest<TResponse>
{
    public Paging Paging { get; init; }
}
