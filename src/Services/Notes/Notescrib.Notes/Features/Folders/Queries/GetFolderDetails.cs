using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Folders.Models;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Features.Notes.Models;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.MongoDb;

namespace Notescrib.Notes.Features.Folders.Queries;

public static class GetFolderDetails
{
    public record Query(string? Id) : IQuery<FolderDetails>;

    internal class Handler : IQueryHandler<Query, FolderDetails>
    {
        private readonly MongoDbContext _context;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IMapper<Folder, FolderDetails> _folderMapper;
        private readonly IMapper<Note, NoteOverview> _noteMapper;

        public Handler(
            MongoDbContext context,
            IPermissionGuard permissionGuard,
            IMapper<Folder, FolderDetails> folderMapper,
            IMapper<Note, NoteOverview> noteMapper)
        {
            _context = context;
            _permissionGuard = permissionGuard;
            _folderMapper = folderMapper;
            _noteMapper = noteMapper;
        }

        public async Task<FolderDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var includeOptions = new FolderIncludeOptions { ImmediateChildren = true };

            var folder = request.Id == null
                ? await _context.Folders.GetRootAsync(_permissionGuard.UserContext.UserId, includeOptions,
                    cancellationToken)
                : await _context.Folders.GetByIdAsync(request.Id, includeOptions, cancellationToken);
            if (folder == null)
            {
                throw new NotFoundException(ErrorCodes.Folder.FolderNotFound);
            }

            _permissionGuard.GuardCanView(folder.OwnerId);

            var notes = await _context.Notes.GetByFolderIdAsync(folder.Id, cancellationToken: cancellationToken);

            var mapped = _folderMapper.Map(folder);
            
            // TODO: Maybe do this on the DB server in the future.
            mapped.Children = mapped.Children.OrderBy(x => x.Name).ToArray();
            mapped.Notes = notes.OrderBy(x => x.Name).Select(_noteMapper.Map).ToArray();

            return mapped;
        }
    }
}
