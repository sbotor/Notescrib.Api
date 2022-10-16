﻿using MediatR;

namespace Notescrib.Api.Application.Cqrs;

internal interface ICommandHandler<in TCommand, TResponse> : IRequestHandler<TCommand, TResponse>
    where TCommand : ICommand<TResponse>
{
}