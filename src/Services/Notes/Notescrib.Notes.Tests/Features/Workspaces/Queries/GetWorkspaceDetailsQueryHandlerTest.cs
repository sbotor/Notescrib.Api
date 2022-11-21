using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Mappers;
using Notescrib.Notes.Services;
using Notescrib.Notes.Tests.Infrastructure;
using static Notescrib.Notes.Features.Workspaces.Queries.GetWorkspaceDetails;

namespace Notescrib.Notes.Tests.Features.Workspaces.Queries;

public class GetWorkspaceDetailsQueryHandlerTest
{
    private static int Counter;
    
    private readonly TestUserContextProvider _userContext = new();
    private readonly TestWorkspaceRepository _repository = new();
    
    private readonly Handler _sut;

    public GetWorkspaceDetailsQueryHandlerTest()
    {
        _sut = new(new PermissionGuard(_userContext), _repository, new WorkspaceDetailsMapper());
        
        _repository.Items.Add(CreateWorkspace("1"));

        _userContext.UserId = "1";
    }

    [Fact]
    public async Task Handle_WhenWorkspaceExistsAndUserIsOwner_ReturnsWorkspace()
    {
        var workspaceId = _repository.Items.First().Id;
        var result = await _sut.Handle(new(workspaceId), default);
        
        Assert.Equal(workspaceId, result.Id);
    }
    
    [Fact]
    public Task Handle_WhenWorkspaceDoesNotExist_ThrowsNotFoundException()
        => Assert.ThrowsAnyAsync<NotFoundException>(
            () => _sut.Handle(new("asdf"), default));

    [Fact]
    public async Task Handle_WhenWorkspaceExistsAndUserIsNotTheOwner_ThrowsForbiddenException()
    {
        _userContext.UserId = "asdf";
        
        var workspaceId = _repository.Items.First().Id;
        await Assert.ThrowsAsync<ForbiddenException>(
            () => _sut.Handle(new(workspaceId), default));
    }
    
    private static Workspace CreateWorkspace(string ownerId)
        => new() { Name = "Workspace " + Counter++, OwnerId = ownerId };
}
