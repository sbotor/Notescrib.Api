using Microsoft.EntityFrameworkCore;
using Notescrib.Contracts;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Data;
using Notescrib.Features.Folders.Extensions;
using Notescrib.Features.Folders.Models;
using Notescrib.Features.Notes.Mappers;
using Notescrib.Features.Notes.Models;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Folders.Queries;

public static class GetFolderDetails
{
    public record Query(Guid? Id) : IQuery<FolderDetails>;

    internal class Handler : IQueryHandler<Query, FolderDetails>
    {
        private readonly NotescribDbContext _dbContext;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IMapper<Folder, FolderDetails> _folderMapper;
        private readonly INoteOverviewMapper _noteMapper;

        public Handler(
            NotescribDbContext dbContext,
            IPermissionGuard permissionGuard,
            IMapper<Folder, FolderDetails> folderMapper,
            INoteOverviewMapper noteMapper)
        {
            _dbContext = dbContext;
            _permissionGuard = permissionGuard;
            _folderMapper = folderMapper;
            _noteMapper = noteMapper;
        }

        public async Task<FolderDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var userId = await _permissionGuard.UserContext.GetUserId(CancellationToken.None);

            var folder = await _dbContext.Folders.AsNoTracking()
                .Include(x => x.Notes.OrderBy(y => y.Name)).ThenInclude(x => x.Tags)
                .Include(x => x.Children.OrderBy(y => y.Name))
                .GetFolderOrRootAsync(userId, request.Id, CancellationToken.None)
                ?? throw new NotFoundException(ErrorCodes.Folder.FolderNotFound);

            await _permissionGuard.GuardCanView(folder.OwnerId);

            var mapped = _folderMapper.Map(folder);

            var mappedNotes = new List<NoteOverview>(folder.Notes.Count);
            mapped.Notes = mappedNotes;

            foreach (var note in folder.Notes)
            {
                var isReadonly = !await _permissionGuard.CanEdit(note.OwnerId);
                mappedNotes.Add(_noteMapper.Map(note, isReadonly));
            }
            
            return mapped;
        }
    }
}
