using FluentValidation;
using MediatR;
using Notescrib.Core.Cqrs;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Extensions;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Services;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Folders.Commands;

public static class UpdateFolder
{
    public record Command(string FolderId, string Name) : ICommand;

    internal class Handler : ICommandHandler<Command>
    {
        private readonly IFolderRepository _folderRepository;
        private readonly IDateTimeProvider _dateTimeProvider;

        public Handler(
            IFolderRepository folderRepository,
            IDateTimeProvider dateTimeProvider)
        {
            _folderRepository = folderRepository;
            _dateTimeProvider = dateTimeProvider;
        }

        public async Task<Unit> Handle(Command request, CancellationToken cancellationToken)
        {
            var folder = await _folderRepository.GetByIdAsync(request.FolderId, cancellationToken: cancellationToken);
            if (folder == null)
            {
                throw new NotFoundException<Folder>(request.FolderId);
            }

            if (folder.ParentId == null)
            {
                throw new AppException("Cannot update root folder.");
            }

            // TODO: optimize this later.
            var parent = await _folderRepository.GetByIdAsync(
                    folder.ParentId,
                    new() { Children = true },
                    cancellationToken);
            if (parent == null)
            {
                throw new NotFoundException<Folder>();
            }

            if (parent.Children.Any(x => x.Name == request.Name))
            {
                throw new DuplicationException<Folder>();
            }

            var now = _dateTimeProvider.Now;
            folder.Updated = now;
            folder.Name = request.Name;

            await _folderRepository.UpdateAsync(folder, cancellationToken);

            return Unit.Value;
        }
    }

    internal class Validator : AbstractValidator<Command>
    {
        public Validator()
        {
            RuleFor(x => x.FolderId)
                .NotEmpty();

            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(Consts.Name.MaxLength);
        }
    }
}
