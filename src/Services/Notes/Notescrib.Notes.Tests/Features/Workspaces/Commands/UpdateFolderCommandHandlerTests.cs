using MongoDB.Driver.Linq;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Services;
using Notescrib.Notes.Tests.Infrastructure;
using static Notescrib.Notes.Features.Workspaces.Commands.UpdateFolder;

namespace Notescrib.Notes.Tests.Features.Workspaces.Commands;

public class UpdateFolderCommandHandlerTests
{
    private readonly TestWorkspaceRepository _repository = new();
    private readonly TestUserContextProvider _userContext = new();

    private readonly Handler _sut;

    public UpdateFolderCommandHandlerTests()
    {
        WorkspaceDataSetup.SetupWorkspace(_repository);
        
        _sut = new(_repository, new PermissionGuard(_userContext), new UtcDateTimeProvider());
    }
    
    [Fact]
    public Task Handle_WhenWorkspaceDoesNotExist_ThrowsNotFoundException()
        => Assert.ThrowsAnyAsync<NotFoundException>(
            () => _sut.Handle(
                new("asdf", "Folder", "Name", "Parent"),
                default));

    [Fact]
    public async Task Handle_WhenUserCannotEditWorkspace_ThrowsForbiddenException()
    {
        _userContext.UserId = "asdf";
        await Assert.ThrowsAnyAsync<ForbiddenException>(
            () => _sut.Handle(
                new("1", "Folder", "Name", "Parent"),
                default));
    }
    
    [Theory]
    [InlineData(null)]
    [InlineData("F0")]
    public async Task Handle_WhenFolderAlreadyExists_ThrowsDuplicationException(string? parentId)
    {
        var workspace = _repository.Items.First();
        var parent = workspace.Folders.First(x => x.Id == "F0");
        var newFolder1 = new Folder { Id = "new1", Name = "New folder" };
        var newFolder2 = new Folder { Id = "new2", Name = "New folder" };
        var folder = workspace.Folders.Skip(1).First();
        
        parent.Children.Add(newFolder1);
        workspace.Folders = workspace.Folders.Append(newFolder2).ToArray();

        await Assert.ThrowsAnyAsync<DuplicationException>(
            () => _sut.Handle(
                new("1", folder.Id, newFolder1.Name, parentId),
                default));
    }
}
