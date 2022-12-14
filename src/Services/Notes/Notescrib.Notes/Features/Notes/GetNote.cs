using FluentValidation;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Notes.Models;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes;

public static class GetNote
{
    public record Query(string Id) : IQuery<NoteDetails>;

    internal class Handler : IQueryHandler<Query, NoteDetails>
    {
        private readonly INoteRepository _noteRepository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IMapper<Note, NoteDetails> _mapper;

        public Handler(INoteRepository noteRepository, IPermissionGuard permissionGuard, IMapper<Note, NoteDetails> mapper)
        {
            _noteRepository = noteRepository;
            _permissionGuard = permissionGuard;
            _mapper = mapper;
        }

        public async Task<NoteDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var note = await _noteRepository.GetByIdAsync(request.Id, new() { Content = true }, cancellationToken);
            if (note == null)
            {
                throw new NotFoundException(ErrorCodes.Note.NoteNotFound);
            }
            
            _permissionGuard.GuardCanView(note.OwnerId, note.SharingInfo);

            var details = _mapper.Map(note);
            details.IsReadonly = !_permissionGuard.CanEdit(note.OwnerId);

            return details;
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
