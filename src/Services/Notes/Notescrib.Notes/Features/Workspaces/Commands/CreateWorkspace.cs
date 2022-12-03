using FluentValidation;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Workspaces.Models;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Workspaces.Commands;

public static class CreateWorkspace
{
    public record Command(string Name) : ICommand<WorkspaceDetails>;

    internal class Handler : ICommandHandler<Command, WorkspaceDetails>
    {
        private readonly IWorkspaceRepository _repository;
        private readonly IUserContextProvider _userContext;
        private readonly IMapper<Workspace, WorkspaceDetails> _mapper;

        public Handler(IWorkspaceRepository repository, IUserContextProvider userContext, IMapper<Workspace, WorkspaceDetails> mapper)
        {
            _repository = repository;
            _userContext = userContext;
            _mapper = mapper;
        }

        public async Task<WorkspaceDetails> Handle(Command request, CancellationToken cancellationToken)
        {
            var ownerId = _userContext.UserId;
            if (ownerId == null)
            {
                throw new AppException("No user context found.");
            }

            if (await _repository.ExistsAsync(ownerId, request.Name, cancellationToken))
            {
                throw new DuplicationException<Workspace>();
            }
            
            var workspace = new Workspace
            {
                Name = request.Name,
                OwnerId = ownerId
            };
            await _repository.AddWorkspaceAsync(workspace, cancellationToken);

            return _mapper.Map(workspace);
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Size.Name.MaxLength);
        }
    }
}
