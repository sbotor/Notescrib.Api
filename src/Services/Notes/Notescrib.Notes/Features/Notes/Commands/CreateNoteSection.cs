using FluentValidation;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.Tree;

namespace Notescrib.Notes.Features.Notes.Commands;

public static class CreateNoteSection
{
    public record Command(string NoteId, string ParentId, string Name) : ICommand<string>;

    internal class Handler : ICommandHandler<Command, string>
    {
        private readonly INoteRepository _repository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(INoteRepository repository, IPermissionGuard permissionGuard, IDateTimeProvider dateTimeProvider)
        {
            _repository = repository;
            _permissionGuard = permissionGuard;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            var note = await _repository.GetByIdAsync(request.NoteId, cancellationToken);
            if (note == null)
            {
                throw new NotFoundException<Note>(request.NoteId);
            }
            
            _permissionGuard.GuardCanEdit(note.OwnerId);

            if (note.SectionCount >= Consts.Note.MaxSectionCount)
            {
                throw new AppException("Maximum section count reached.");
            }

            var id = Guid.NewGuid().ToString();
            
            var tree = new Tree<NoteSection>(note.SectionTree);
            var found = tree.VisitBreadthFirst(x =>
            {
                if (x.Item.Id != request.ParentId)
                {
                    return false;
                }

                if (x.Level >= Consts.Note.MaxNestingLevel)
                {
                    throw new AppException("Maximum nesting level reached.");
                }
                
                x.Item.Children.Add(new()
                {
                    Id = id,
                    Name = request.Name
                });
                
                note.SectionCount++;
                note.Updated = _dateTimeProvider.Now;
                
                return true;
            });

            if (!found)
            {
                throw new NotFoundException<NoteSection>(request.ParentId);
            }

            await _repository.UpdateAsync(note, cancellationToken);

            return id;
        }
    }
    
    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.NoteId)
                .NotEmpty();

            RuleFor(x => x.ParentId)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty();
        }
    }
}
