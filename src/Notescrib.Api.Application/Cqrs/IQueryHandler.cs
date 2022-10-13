using MediatR;

namespace Notescrib.Api.Application.Cqrs;

internal interface IQueryHandler<in TQuery, TResponse> : IRequestHandler<TQuery, TResponse>
    where TQuery : IQuery<TResponse>
{
}
