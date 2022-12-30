using Moq;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Notes.Data.MongoDb;
using Notescrib.Notes.Features.Folders;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Repositories;
using Notescrib.Notes.Tests.Infrastructure;
using Notescrib.Notes.Tests.Infrastructure.Extensions;
using static Notescrib.Notes.Features.Workspaces.Commands.CreateWorkspace;

namespace Notescrib.Notes.Tests.Features.Workspaces.Commands;

public class CreateWorkspaceCommandHandlerTests
{
    private readonly List<Workspace> _workspaces = new();
    private readonly List<Folder> _folders = new();

    private readonly Handler _sut;

    public CreateWorkspaceCommandHandlerTests()
    {
        var context = new TestMongoDbContext();
        var mocks = context.Mocks;
        
        mocks.Workspaces.Setup(x => x.AddAsync(It.IsAny<Workspace>(), It.IsAny<CancellationToken>()))
            .Callback((Workspace w, CancellationToken _) => _workspaces.Add(w));
        mocks.Workspaces.Setup(x => x.GetByOwnerIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => _workspaces.FirstOrDefault(x => x.OwnerId == "1"));
        
        mocks.Folders.Setup(x => x.CreateAsync(It.IsAny<Folder>(), It.IsAny<CancellationToken>()))
            .Callback((Folder f, CancellationToken _) => _folders.Add(f));

        var userContextProviderMock = new Mock<IUserContextProvider>();
        userContextProviderMock.SetupUserId("1");
        
        _sut = new(context, userContextProviderMock.Object, new UtcDateTimeProvider());
    }

    [Fact]
    public async Task Handle_WhenUserDoesNotHaveAWorkspace_CreatesNewWorkspace()
    {
        await _sut.Handle(new(), default);
        
        Assert.Single(_workspaces);
        Assert.True(_workspaces[0].OwnerId == "1");
        
        Assert.Single(_folders);
        Assert.True(_folders[0].OwnerId == "1");
        Assert.True(_folders[0].Name == Folder.RootName);
        Assert.True(_folders[0].ParentId == null);
    }

    [Fact]
    public async Task Handle_WhenUserWorkspaceExists_ThrowsDuplicationException()
    {
        _workspaces.Add(new()
        {
            OwnerId = "1"
        });

        await Assert.ThrowsAsync<DuplicationException>(
            () => _sut.Handle(new(), default));
    }
}
