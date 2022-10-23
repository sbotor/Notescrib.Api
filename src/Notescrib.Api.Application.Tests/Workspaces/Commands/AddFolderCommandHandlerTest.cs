using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Workspaces;
using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Tests.Workspaces.Commands;

public class AddFolderCommandHandlerTest
{
    private readonly InMemoryPersistenceProvider<Workspace> _workspaces;
    private readonly InMemoryPersistenceProvider<Folder> _folders;

    private readonly AddFolder.Handler _sut;
    private readonly TestUserContextService _userContextService = TestUserContextService.First;

    public AddFolderCommandHandlerTest()
    {
        _workspaces = new(new List<Workspace>
        {
            new()
            {
                Id = Id.Workspace.First,
                OwnerId = Id.User.First,
                SharingDetails = new()
                {
                    Visibility = Core.Enums.VisibilityLevel.Hidden,
                    AllowedUserIds = new[] { Id.User.Second }
                }
            }
        });

        _folders = new(new List<Folder>());

        _sut = new(
            new WorkspaceRepository(_workspaces),
            new FolderRepository(_folders),
            new PermissionService(_userContextService),
            new FolderMapper());
    }

    [Fact]
    public async Task Handle_WhenFolderDoesNotExist_CreatesNewFolder()
    {
        var command = new AddFolder.Command(Id.Workspace.First, null, "Folder", new());

        var result = await _sut.Handle(command, default);
        var response = result.Response;

        Assert.True(result.IsSuccessful);
        Assert.NotNull(response);

        Assert.Single(_folders.Collection);
        Assert.Equal(command.Name, _folders.Collection.First().Name);
    }
}
