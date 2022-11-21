using FluentValidation;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Workspaces.Models;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Workspaces.Queries;

public static class GetWorkspaceDetails
{
    public record Query(string Id) : IQuery<WorkspaceDetails>;

    internal class Handler : IQueryHandler<Query, WorkspaceDetails>
    {
        private readonly IPermissionGuard _permissionGuard;
        private readonly IWorkspaceRepository _repository;
        private readonly IMapper<Workspace, WorkspaceDetails> _mapper;

        public Handler(IPermissionGuard permissionGuard, IWorkspaceRepository repository, IMapper<Workspace, WorkspaceDetails> mapper)
        {
            _permissionGuard = permissionGuard;
            _repository = repository;
            _mapper = mapper;
        }
        
        public async Task<WorkspaceDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var workspace = await _repository.GetWorkspaceByIdAsync(request.Id, cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException<Workspace>();
            }
            
            _permissionGuard.GuardCanView(workspace.OwnerId);

            return _mapper.Map(workspace);
        }
    }

    internal class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}
