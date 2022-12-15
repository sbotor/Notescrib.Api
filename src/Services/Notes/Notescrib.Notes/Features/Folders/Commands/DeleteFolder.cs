﻿using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Folders.Commands;

public static class DeleteFolder
{
    public record Command(string Id) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IFolderRepository _folderRepository;
        private readonly INoteContentRepository _noteContentRepository;
        private readonly IPermissionGuard _permissionGuard;

        public Handler(IFolderRepository folderRepository, INoteContentRepository noteContentRepository, IPermissionGuard permissionGuard)
        {
            _folderRepository = folderRepository;
            _noteContentRepository = noteContentRepository;
            _permissionGuard = permissionGuard;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = await _folderRepository.GetByIdAsync(
                request.Id,
                new() { Children = true},
                cancellationToken);
            if (folder == null)
            {
                throw new NotFoundException(ErrorCodes.Folder.FolderNotFound);
            }
            
            _permissionGuard.GuardCanEdit(folder.OwnerId);

            if (folder.ParentId == null)
            {
                throw new AppException("Cannot delete root folder.");
            }

            var allFolders = folder.Children.Append(folder).ToArray();
            
            var folderIds = allFolders.Select(x => x.Id);
            var noteIds = allFolders.SelectMany(x => x.Notes.Select(n => n.Id));

            await _folderRepository.DeleteManyAsync(folderIds, CancellationToken.None);
            await _noteContentRepository.DeleteManyAsync(noteIds, CancellationToken.None);

            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.Id)
                .NotEmpty();
        }
    }
}