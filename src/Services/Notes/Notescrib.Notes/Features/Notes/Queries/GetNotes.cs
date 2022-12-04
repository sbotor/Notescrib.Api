using System.Linq.Expressions;
using FluentValidation;
using MongoDB.Driver;
using Notescrib.Core.Cqrs;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Notes.Models;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Notes.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Notes.Queries;

public static class GetNotes
{
    public record Query(string? WorkspaceId, string? FolderId, Paging Paging, Sorting<NotesSorting> Sorting)
        : IPagingSortingQuery<NoteOverview, NotesSorting>;

    internal class Handler : IQueryHandler<Query, PagedList<NoteOverview>>
    {
        private readonly INoteRepository _repository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IMapper<Note, NoteOverview> _mapper;
        private readonly ISortingProvider<NotesSorting> _sortingProvider;

        public Handler(
            INoteRepository repository,
            IPermissionGuard permissionGuard,
            IMapper<Note, NoteOverview> mapper,
            ISortingProvider<NotesSorting> sortingProvider)
        {
            _repository = repository;
            _permissionGuard = permissionGuard;
            _mapper = mapper;
            _sortingProvider = sortingProvider;
        }

        public async Task<PagedList<NoteOverview>> Handle(Query request, CancellationToken cancellationToken)
        {
            var notes = await _repository.GetNotesAsync(
                request.WorkspaceId,
                request.FolderId,
                _permissionGuard,
                new(request.Paging, request.Sorting, _sortingProvider),
                cancellationToken);

            return notes.Map(_mapper.Map);
        }
    }

    internal class Validator : AbstractValidator<Query>
    {
        public Validator()
        {
            RuleFor(x => x.WorkspaceId)
                .NotEmpty()
                .When(x => x.WorkspaceId != null || string.IsNullOrEmpty(x.FolderId));

            RuleFor(x => x.FolderId)
                .NotEmpty()
                .When(x => x.FolderId != null);
        }
    }
}
