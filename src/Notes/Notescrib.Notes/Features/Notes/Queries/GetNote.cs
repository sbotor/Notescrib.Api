using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Notes.Models;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Notes.Queries;

public static class GetNote
{
    public record Query(string Id) : IQuery<NoteDetails>;

    internal class Handler : IQueryHandler<Query, NoteDetails>
    {
        private readonly INoteRepository _repository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IMapper<Note, NoteDetails> _mapper;

        public Handler(INoteRepository repository, IPermissionGuard permissionGuard, IMapper<Note, NoteDetails> mapper)
        {
            _repository = repository;
            _permissionGuard = permissionGuard;
            _mapper = mapper;
        }

        public async Task<NoteDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var found = await _repository.GetByIdAsync(request.Id, cancellationToken);
            if (found == null)
            {
                throw new NotFoundException<Note>();
            }
            
            _permissionGuard.GuardCanView(found.OwnerId, found.SharingInfo);

            return _mapper.Map(found);
        }
    }
}
