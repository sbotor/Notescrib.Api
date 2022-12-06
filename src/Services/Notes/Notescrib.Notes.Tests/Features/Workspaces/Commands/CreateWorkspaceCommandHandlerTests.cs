using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Mappers;
using Notescrib.Notes.Services;
using Notescrib.Notes.Tests.Infrastructure;
using Notescrib.Notes.Utils;
using static Notescrib.Notes.Features.Workspaces.Commands.CreateWorkspace;

namespace Notescrib.Notes.Tests.Features.Workspaces.Commands;

public class CreateWorkspaceCommandHandlerTests
{
    private readonly TestUserContextProvider _userContext = new();
    private readonly TestWorkspaceRepository _repository = new();

    private readonly Handler _sut;

    public CreateWorkspaceCommandHandlerTests()
    {
        _sut = new(_repository, _userContext, new WorkspaceDetailsMapper(), new UtcDateTimeProvider());
    }

    [Fact]
    public async Task Handle_WhenMaxCountNotReached_CreatesNewWorkspace()
    {
        _userContext.UserId = "1";

        await _sut.Handle(new("Workspace"), default);

        Assert.Single(_repository.Items);
        Assert.True(_repository.Items.First().OwnerId == "1");
    }

    [Fact]
    public async Task Handle_WhenMaxCountReached_ThrowsAppException()
    {
        _userContext.UserId = "1";

        foreach (var i in Enumerable.Range(1, Counts.Workspace.MaxCount))
        {
            _repository.Items.Add(new Workspace { Name = "name", Id = i.ToString(), OwnerId = "1" });
        }

        await Assert.ThrowsAsync<AppException>(() => _sut.Handle(new("Workspace"), default));
    }
}
