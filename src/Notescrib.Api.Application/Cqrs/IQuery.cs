using MediatR;

namespace Notescrib.Api.Application.Cqrs;

internal interface IQuery<out TResponse> : IRequest<TResponse>
{
}
