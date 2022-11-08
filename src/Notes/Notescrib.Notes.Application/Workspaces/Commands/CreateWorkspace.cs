using MediatR;
using Notescrib.Notes.Application.Common.Providers;
using Notescrib.Notes.Application.Workspaces.Mappers;
using Notescrib.Notes.Core.Entities;
using Notescrib.Notes.Core.Exceptions;

namespace Notescrib.Notes.Application.Workspaces.Commands;

public static class CreateWorkspace
{
    public record Command(string Name, SharingInfo SharingInfo) : IRequest<string>;

    internal class Handler : IRequestHandler<Command, string>
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

            var workspace = _mapper.MapToEntity(request, ownerId);
            return (await _repository.AddAsync(workspace)).Id;
        }
    }
}
