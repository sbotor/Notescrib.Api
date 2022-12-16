using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes.Commands;

public static class UpdateNoteContent
{
    public record Command(string NoteId, string Content) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly INoteContentRepository _noteContentRepository;
        private readonly IFolderRepository _folderRepository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(INoteContentRepository noteContentRepository, IFolderRepository folderRepository, IPermissionGuard permissionGuard, IDateTimeProvider dateTimeProvider)
        {
            _noteContentRepository = noteContentRepository;
            _folderRepository = folderRepository;
            _permissionGuard = permissionGuard;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var content = await _noteContentRepository.GetByNoteIdAsync(request.NoteId, cancellationToken);
            if (content == null)
            {
                throw new NotFoundException(ErrorCodes.Note.NoteNotFound);
            }
            
            _permissionGuard.GuardCanEdit(content.Note.OwnerId);

            content.Value = request.Content;
            content.Note.Updated = _dateTimeProvider.Now;

            await _noteContentRepository.UpdateAsync(content, cancellationToken);
            await _folderRepository.UpdateNoteAsync(content.Note, CancellationToken.None);

            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.NoteId)
                .NotEmpty();

            RuleFor(x => x.Content)
                .MaximumLength(Consts.Note.MaxContentLength);
        }
    }
}
