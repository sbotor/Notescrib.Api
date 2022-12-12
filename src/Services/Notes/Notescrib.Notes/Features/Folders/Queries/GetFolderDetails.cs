using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Features.Folders.Models;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Services;

namespace Notescrib.Notes.Features.Folders.Queries;

public static class GetFolderDetails
{
    public record Query(string? Id) : IQuery<FolderDetails>;

    internal class Handler : IQueryHandler<Query, FolderDetails>
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IPermissionGuard _permissionGuard;
        private readonly IMapper<Folder, FolderDetails> _folderMapper;

        public Handler(
            IFolderRepository folderRepository,
            IPermissionGuard permissionGuard,
            IMapper<Folder, FolderDetails> folderMapper)
        {
            _folderRepository = folderRepository;
            _permissionGuard = permissionGuard;
            _folderMapper = folderMapper;
        }

        public async Task<FolderDetails> Handle(Query request, CancellationToken cancellationToken)
        {
            var includeOptions = new FolderIncludeOptions { ImmediateChildren = true, ImmediateNotes = true };

            var folder = request.Id == null
                ? await _folderRepository.GetRootAsync(_permissionGuard.UserContext.UserId, includeOptions,
                    cancellationToken)
                : await _folderRepository.GetByIdAsync(request.Id, includeOptions, cancellationToken);
            if (folder == null)
            {
                throw new NotFoundException<Folder>(request.Id);
            }

            _permissionGuard.GuardCanView(folder.OwnerId);

            folder.ImmediateNotes = folder.ImmediateNotes
                .Where(x => _permissionGuard.CanView(x.OwnerId, x.SharingInfo))
                .ToArray();

            return _folderMapper.Map(folder);
        }
    }
}
