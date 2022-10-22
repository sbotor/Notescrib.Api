using Moq;
using Notescrib.Api.Application.Common;
using Notescrib.Api.Application.Workspaces;
using Notescrib.Api.Application.Workspaces.Commands;
using Notescrib.Api.Application.Workspaces.Mappers;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Tests.Workspaces.Commands;

public class AddFolderCommandHandlerTest
{
    private readonly InMemoryPersistenceProvider<Workspace> _workspaces;

    private readonly AddFolder.Handler _sut;
    private readonly TestUserContextService _userContextService = TestUserContextService.First;

    public AddFolderCommandHandlerTest()
    {
        _workspaces = new(new List<Workspace>
        {
            new Workspace
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

        _sut = new(
            new WorkspaceRepository(_workspaces),
            new PermissionService(_userContextService),
            new FolderMapper());
    }

    [Fact]
    public async Task Handle_WhenFolderDoesNotExist_CreatesNewFolder()
    {
        var workspace = _workspaces.Collection.First();
        var command = new AddFolder.Command(Id.Workspace.First, null, "Folder", new());

        var result = await _sut.Handle(command, default);
        var response = result.Response;

        Assert.True(result.IsSuccessful);
        Assert.NotNull(response);

        Assert.Single(workspace.Folders);
        Assert.Equal(command.Name, workspace.Folders.First().Name);
    }
}
