using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Mappers;
using Notescrib.Notes.Features.Workspaces.Utils;
using Notescrib.Notes.Models;
using Notescrib.Notes.Tests.Infrastructure;
using static Notescrib.Notes.Features.Workspaces.Queries.GetWorkspaces;

namespace Notescrib.Notes.Tests.Features.Workspaces.Queries;

public class GetWorkspacesQueryHandlerTests
{
    private static int Counter = 0;

    private readonly TestWorkspaceRepository _repository = new();
    private readonly TestUserContextProvider _userContext = new();

    private readonly Handler _sut;

    public GetWorkspacesQueryHandlerTests()
    {
        _sut = new(_repository, _userContext, new WorkspaceMapper(), new WorkspacesSortingProvider());
    }

    [Fact]
    public async Task Handle_Always_ReturnsCorrectWorkspaces()
    {
        _userContext.UserId = "1";

        _repository.Items.AddRange(new[]
        {
            CreateWorkspace("1"),
            CreateWorkspace("2"),
            CreateWorkspace("1"),
            CreateWorkspace("1"),
            CreateWorkspace("3"),
        });

        var result = await _sut.Handle(new(new Paging(1, 4), new()), default);
        
        Assert.Equal(3, result.TotalCount);
        Assert.Equal(3, result.Data.Count);
        Assert.All(result.Data, x => Assert.Equal("1", x.OwnerId));
    }

    private static Workspace CreateWorkspace(string ownerId)
        => new() { Name = "Workspace " + Counter++, OwnerId = ownerId };
}
