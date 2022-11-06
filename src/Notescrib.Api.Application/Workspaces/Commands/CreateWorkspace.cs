using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Cqrs;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Exceptions;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Workspaces.Commands;

public static class CreateWorkspace
{
    public record Command(string Name, SharingInfo SharingInfo) : ICommand<string>;

    internal class Handler : ICommandHandler<Command, string>
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

        public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            var ownerId = _userContext.UserId;
            if (ownerId == null)
            {
                throw new AppException("No user context found.");
            }

            var workspace = _mapper.CreateEntity(request, ownerId);
            return await _repository.AddAsync(workspace);
        }
    }
}
