﻿using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

public static class CreateWorkspace
{
    public record Command(string Name, SharingInfo SharingInfo) : ICommand<Result<string>>;

    internal class Handler : ICommandHandler<Command, Result<string>>
    {
        private readonly IUserContextProvider _userContext;
        private readonly IWorkspaceRepository _repository;
        private readonly IWorkspaceMapper _mapper;

        public Handler(IUserContextProvider userContext, IWorkspaceRepository repository, IWorkspaceMapper mapper)
        {
            _userContext = userContext;
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Result<string>> Handle(Command request, CancellationToken cancellationToken)
        {
            var ownerId = _userContext.UserId;
            if (ownerId == null)
            {
                return Result<string>.Failure("No user context found.");
            }

            var workspace = _mapper.CreateEntity(request, ownerId);
            await _repository.AddAsync(workspace);

            return Result<string>.Created(workspace.Id);
        }
    }
}