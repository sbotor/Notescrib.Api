using FluentValidation;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Notes.Models;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes.Queries;

public static class GetNote
{
    public record Query(string Id) : IQuery<NoteDetails>;

    internal class Handler : IQueryHandler<Query, NoteDetails>
    {
        private readonly INoteContentRepository _noteContentRepository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IMapper<NoteContent, NoteDetails> _mapper;

        public Handler(INoteContentRepository noteContentRepository, IPermissionGuard permissionGuard, IMapper<NoteContent, NoteDetails> mapper)
        {
            _noteContentRepository = noteContentRepository;
            _permissionGuard = permissionGuard;
            _mapper = mapper;
        }

        public async Task<NoteDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var content = await _noteContentRepository.GetByNoteIdAsync(request.Id, cancellationToken);
            if (content == null)
            {
                throw new NotFoundException(ErrorCodes.Note.NoteNotFound);
            }

            var note = content.Note;
            _permissionGuard.GuardCanView(note.OwnerId, note.SharingInfo);

            var details = _mapper.Map(content);
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
