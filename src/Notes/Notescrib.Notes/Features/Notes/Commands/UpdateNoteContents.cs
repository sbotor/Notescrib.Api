using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Notes.Models;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Notes.Utils;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Notes.Commands;

public static class UpdateNoteContents
{
    public record Command(string Id, IEnumerable<NoteContentsSection> Sections) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly INoteRepository _repository;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(INoteRepository repository, IPermissionGuard permissionGuard)
        {
            _repository = repository;
            _permissionGuard = permissionGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var note = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (note == null)
            {
                throw new NotFoundException<Note>();
            }

            _permissionGuard.GuardCanEdit(note.WorkspaceId);

            var tree = MapContents(request.Sections);
            tree.ValidateAndThrow();

            note.Contents = tree.Roots.ToList();

            await _repository.UpdateNoteAsync(note, cancellationToken);
            return Unit.Value;
        }

        private static NoteSectionTree MapContents(IEnumerable<NoteContentsSection> original)
            => new(original
                .MapTree(x => new NoteSection
                {
                    Name = x.Name,
                    Content = x.Content,
                    Children = new List<NoteSection>()
                }).ToList());
    }
}
