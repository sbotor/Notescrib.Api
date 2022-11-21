using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Tests.Infrastructure;
using static Notescrib.Notes.Features.Workspaces.Commands.CreateWorkspace;

namespace Notescrib.Notes.Tests.Features.Workspaces.Commands;

public class CreateWorkspaceCommandHandlerTests
{
    private readonly TestUserContextProvider _userContext = new();
    private readonly TestWorkspaceRepository _repository = new();

    private readonly Handler _sut;
    
    public CreateWorkspaceCommandHandlerTests()
    {
        _sut = new(_repository, _userContext);
    }

    [Fact]
    public async Task Handle_WhenWorkspaceDoesNotExist_CreatesNewWorkspace()
    {
        _userContext.UserId = "1";

        await _sut.Handle(new("Workspace"), default);

        Assert.Single(_repository.Items);
        Assert.True(_repository.Items.First().OwnerId == "1");
    }
    
    [Fact]
    public async Task Handle_WhenWorkspaceExists_ThrowsDuplicationException()
    {
        _repository.Items.Add(new()
        {
            Name = "Workspace",
            OwnerId = "1"
        });
        
        _userContext.UserId = "1";

        await Assert.ThrowsAnyAsync<DuplicationException>(
            () => _sut.Handle(new("Workspace"), default));
    }
}
