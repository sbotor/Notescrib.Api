using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Services;
using Notescrib.Notes.Tests.Infrastructure;
using static Notescrib.Notes.Features.Folders.Commands.UpdateFolder;

namespace Notescrib.Notes.Tests.Features.Workspaces.Commands;

public class UpdateFolderCommandHandlerTests
{
    private readonly TestWorkspaceRepository _repository = new();
    private readonly TestUserContextProvider _userContext = new();

    private readonly Handler _sut;

    public UpdateFolderCommandHandlerTests()
    {
        WorkspaceDataSetup.SetupWorkspace(_repository);
        
        _sut = new(_repository, new PermissionGuard(_userContext), new UtcDateTimeProvider(), _userContext);
    }
    
    [Fact]
    public Task Handle_WhenWorkspaceDoesNotExist_ThrowsNotFoundException()
        => Assert.ThrowsAnyAsync<NotFoundException>(
            () => _sut.Handle(
                new("Folder", "Name", "Parent"),
                default));

    [Fact]
    public async Task Handle_WhenUserCannotEditWorkspace_ThrowsForbiddenException()
    {
        _userContext.UserId = "asdf";
        await Assert.ThrowsAnyAsync<ForbiddenException>(
            () => _sut.Handle(
                new("Folder", "Name", "Parent"),
                default));
    }
}
