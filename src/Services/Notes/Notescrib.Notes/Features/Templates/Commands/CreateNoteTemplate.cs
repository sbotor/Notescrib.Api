using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Templates.Repositories;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Templates.Commands;

public static class CreateNoteTemplate
{
    public record Command(string Name) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly INoteTemplateRepository _repository;
        private readonly IWorkspaceRepository _workspaceRepository;
        private readonly IUserContextProvider _userContextProvider;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(INoteTemplateRepository repository, IWorkspaceRepository workspaceRepository,
            IUserContextProvider userContextProvider, IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            _workspaceRepository = workspaceRepository;
            _userContextProvider = userContextProvider;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var userId = _userContextProvider.UserId;
            var workspace = await _workspaceRepository.GetByOwnerIdAsync(userId, cancellationToken);
            if (workspace == null)
            {
                throw new NotFoundException(ErrorCodes.Workspace.WorkspaceNotFound);
            }

            var template = new NoteTemplate
            {
                Name = request.Name,
                OwnerId = userId,
                WorkspaceId = workspace.Id,
                Created = _dateTimeProvider.Now,
                Content = string.Empty
            };

            await _repository.CreateAsync(template, cancellationToken);

            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Consts.Name.MaxLength);
        }
    }
}
