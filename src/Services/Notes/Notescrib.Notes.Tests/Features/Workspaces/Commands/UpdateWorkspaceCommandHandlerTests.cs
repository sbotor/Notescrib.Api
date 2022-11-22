using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Services;
using Notescrib.Notes.Tests.Infrastructure;
using static Notescrib.Notes.Features.Workspaces.Commands.UpdateWorkspace;

namespace Notescrib.Notes.Tests.Features.Workspaces.Commands;

public class UpdateWorkspaceCommandHandlerTests
{
    private readonly TestWorkspaceRepository _repository = new();
    private readonly TestUserContextProvider _userContext = new();

    private readonly Handler _sut;
    
    public UpdateWorkspaceCommandHandlerTests()
    {
        WorkspaceDataSetup.SetupWorkspace(_repository);
        
        _sut = new(_repository, new PermissionGuard(_userContext));
    }

    [Fact]
    public Task Handle_WhenWorkspaceDoesNotExist_ThrowsNotFoundException()
        => Assert.ThrowsAnyAsync<NotFoundException>(
            () => _sut.Handle(new("asdf", "Name"), default));

    [Fact]
    public async Task Handle_WhenUserCannotEditWorkspace_ThrowsForbiddenException()
    {
        _userContext.UserId = "asdf";
        await Assert.ThrowsAnyAsync<ForbiddenException>(
            () => _sut.Handle(new("1", "Name"), default));
    }

    [Fact]
    public async Task Handle_WhenWorkspaceAlreadyExists_ThrowsDuplicationException()
    {
        var newWorkspace = new Workspace() { Name = "Name", OwnerId = "1" };
        _repository.Items.Add(newWorkspace);
        
        await Assert.ThrowsAnyAsync<DuplicationException>(
            () => _sut.Handle(new("1", newWorkspace.Name), default));
    }

    [Fact]
    public async Task Handle_ForValidData_UpdatesWorkspace()
    {
        await _sut.Handle(new("1", "Changed name"), default);

        var changed = _repository.Items.First();
        
        Assert.Equal("Changed name", changed.Name);
    }
}
