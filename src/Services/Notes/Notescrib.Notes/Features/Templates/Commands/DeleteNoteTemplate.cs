using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Templates.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Templates.Commands;

public static class DeleteNoteTemplate
{
    public record Command(string Id) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly INoteTemplateRepository _repository;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(INoteTemplateRepository repository, IPermissionGuard permissionGuard)
        {
            _repository = repository;
            _permissionGuard = permissionGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var template = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (template == null)
            {
                throw new NotFoundException(ErrorCodes.NoteTemplate.NoteTemplateNotFound);
            }
            
            _permissionGuard.GuardCanEdit(template.OwnerId);

            await _repository.DeleteAsync(template.Id, CancellationToken.None);

            return Unit.Value;
        }
    }
}
