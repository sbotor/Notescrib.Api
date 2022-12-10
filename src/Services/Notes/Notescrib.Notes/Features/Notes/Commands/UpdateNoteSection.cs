﻿using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.Tree;

namespace Notescrib.Notes.Features.Notes.Commands;

public static class UpdateNoteSection
{
    public record Command(string NoteId, string SectionId, string Name, string Content) : ICommand;

    internal class Handler : ICommandHandler<Command>
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

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var note = await _repository.GetByIdAsync(request.NoteId, cancellationToken);
            if (note == null)
            {
                throw new NotFoundException<Note>(request.NoteId);
            }

            _permissionGuard.GuardCanEdit(note.OwnerId);

            var tree = new Tree<NoteSection>(note.SectionTree);
            var found = tree.VisitBreadthFirst(x =>
            {
                if (x.Item.Id != request.SectionId)
                {
                    return false;
                }

                x.Item.Name = request.Name;
                x.Item.Content = request.Content;

                note.Updated = _dateTimeProvider.Now;
                
                return true;
            });

            if (!found)
            {
                throw new NotFoundException<NoteSection>(request.SectionId);
            }

            await _repository.UpdateAsync(note, cancellationToken);
            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.NoteId)
                .NotEmpty();
            
            RuleFor(x => x.NoteId)
                .NotEmpty()
                .NotEqual(NoteSection.RootId);

            RuleFor(x => x.Name)
                .NotEmpty();

            RuleFor(x => x.Content.Length)
                .LessThanOrEqualTo(Consts.Note.MaxSectionLength);
        }
    }
}
