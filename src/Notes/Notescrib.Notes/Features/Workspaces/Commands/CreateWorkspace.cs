using MediatR;
using MongoDB.Driver;
using Notescrib.Notes.Application.Models;
using Notescrib.Notes.Application.Models.Exceptions;
using Notescrib.Notes.Application.Services;

namespace Notescrib.Notes.Application.Features.Workspaces.Commands;

public static class AddWorkspace
{
    public record Command(string Name, SharingInfo? SharingInfo) : IRequest<string>;

    internal class Handler : IRequestHandler<Command, string>
    {
        private readonly IMongoCollection<Workspace> _collection;
        private readonly IUserContextProvider _userContext;

        public Handler(IMongoCollection<Workspace> collection, IUserContextProvider userContext)
        {
            _collection = collection;
            _userContext = userContext;
        }

        public async Task<string> Handle(Command request, CancellationToken cancellationToken)
        {
            var ownerId = _userContext.UserId;
            if (ownerId == null)
            {
                throw new AppException("No user context found.");
            }

            var workspace = new Workspace
            {
                Name = request.Name,
                OwnerId = ownerId,
                SharingInfo = request.SharingInfo ?? new()
            };
            await _collection.InsertOneAsync(workspace, cancellationToken: cancellationToken);

            return workspace.Id;
        }
    }
}
