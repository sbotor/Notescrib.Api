using Notescrib.Contracts;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Data.MongoDb;
using Notescrib.Features.Folders.Models;
using Notescrib.Features.Folders.Repositories;
using Notescrib.Features.Notes;
using Notescrib.Features.Notes.Models;
using Notescrib.Services;
using Notescrib.Utils;

namespace Notescrib.Features.Folders.Queries;

public static class GetFolderDetails
{
    public record Query(string? Id) : IQuery<FolderDetails>;

    internal class Handler : IQueryHandler<Query, FolderDetails>
    {
        private readonly IMongoDbContext _context;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IMapper<Folder, FolderDetails> _folderMapper;
        private readonly IMapper<Note, NoteOverview> _noteMapper;

        public Handler(
            IMongoDbContext context,
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
                ? await _context.Folders.GetRootAsync(includeOptions, cancellationToken)
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
