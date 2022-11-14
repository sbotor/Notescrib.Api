using System.Linq.Expressions;
using MediatR;
using MongoDB.Driver;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Notes.Models;
using Notescrib.Notes.Features.Notes.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Notes.Commands;

public static class GetNotes
{
    public record Query(string? WorkspaceId, string? Folder, Paging Paging, Sorting<NotesSorting> Sorting)
        : IPagingSortingRequest<NoteOverview, NotesSorting>;

    internal class Handler : IRequestHandler<Query, PagedList<NoteOverview>>
    {
        private readonly IMongoCollection<Note> _collection;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IMapper<Note, NoteOverview> _mapper;
        private readonly ISortingProvider<NotesSorting> _sortingProvider;

        public Handler(
            IMongoCollection<Note> collection,
            IPermissionGuard permissionGuard,
            IMapper<Note, NoteOverview> mapper,
            ISortingProvider<NotesSorting> sortingProvider)
        {
            _collection = collection;
            _permissionGuard = permissionGuard;
            _mapper = mapper;
            _sortingProvider = sortingProvider;
        }

        public async Task<PagedList<NoteOverview>> Handle(Query request, CancellationToken cancellationToken)
        {
            var filters = new Expression<Func<Note, bool>>[]
            {
                x => (request.WorkspaceId == null || x.WorkspaceId == request.WorkspaceId)
                     && (request.Folder == null || x.Folder == request.Folder),
                _permissionGuard.ExpressionCanView<Note>()
            };
            
            var notes = await _collection
                .FindPagedAsync(
                    filters,
                    request.Paging,
                    request.Sorting,
                    _sortingProvider);

            return notes.Map(_mapper.Map);
        }
    }
}
