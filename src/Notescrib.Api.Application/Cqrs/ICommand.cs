using MediatR;

namespace Notescrib.Api.Application.Cqrs;

internal interface ICommand<out TResponse> : IRequest<TResponse>
{
}

internal interface ICommand : IRequest
{
}
