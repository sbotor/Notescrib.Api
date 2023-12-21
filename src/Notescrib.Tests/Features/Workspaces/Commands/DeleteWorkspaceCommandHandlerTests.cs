using Moq;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Features.Folders;
using Notescrib.Features.Notes;
using Notescrib.Features.Templates;
using Notescrib.Features.Workspaces;
using Notescrib.Tests.Infrastructure;
using Notescrib.Tests.Infrastructure.Extensions;
using static Notescrib.Features.Workspaces.Commands.DeleteWorkspace;

namespace Notescrib.Tests.Features.Workspaces.Commands;

public class DeleteWorkspaceCommandHandlerTests
{
    private readonly List<Workspace> _workspaces = new();
    private readonly List<Folder> _folders = new();
    private readonly List<Note> _notes = new();
    private readonly List<NoteTemplate> _templates = new();

    private readonly Mock<IUserContextProvider> _userContextProviderMock = new();

    private readonly Handler _sut;

    public DeleteWorkspaceCommandHandlerTests()
    {
        var context = new TestMongoDbContext();
        var mocks = context.Mocks;

        _userContextProviderMock.SetupUserId("1");

        mocks.Workspaces.Setup(x => x.DeleteAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
            .Callback((string id, CancellationToken _) => _workspaces.Remove(_workspaces.First(x => x.Id == id)));
        mocks.Workspaces.Setup(x => x.GetByOwnerIdAsync(It.IsAny<CancellationToken>()))
            .ReturnsAsync(() => _workspaces.FirstOrDefault(x => x.OwnerId == _userContextProviderMock.Object.UserId));

        mocks.Folders.Setup(x => x.DeleteAllAsync(It.IsAny<CancellationToken>()))
            .Callback((CancellationToken _) => _folders.RemoveAll(x => x.OwnerId == "1"));

        mocks.Notes.Setup(x => x.DeleteAllAsync(It.IsAny<CancellationToken>()))
            .Callback((CancellationToken _) => _notes.RemoveAll(x => x.OwnerId == "1"));

        mocks.NoteTemplates.Setup(x => x.DeleteAllAsync(It.IsAny<CancellationToken>()))
            .Callback((CancellationToken _) => _templates.RemoveAll(x => x.OwnerId == "1"));

        _sut = new(context);

        SetupData();
    }

    [Fact]
    public async Task Handle_WhenWorkspaceExists_DeletesAllData()
    {
        await _sut.Handle(new(), default);

        Assert.Collection(_workspaces, x => Assert.True(x.OwnerId == "2"));
        Assert.Collection(_folders, x => Assert.True(x.OwnerId == "2"));
        Assert.Collection(_notes, x => Assert.True(x.OwnerId == "2"));
        Assert.Collection(_templates, x => Assert.True(x.OwnerId == "2"));
    }

    [Fact]
    public async Task Handle_WhenWorkspaceDoesNotExist_ThrowsNotFoundException()
    {
        _userContextProviderMock.SetupUserId("100");
        await Assert.ThrowsAsync<NotFoundException>(() => _sut.Handle(new(), default));
    }

    private void SetupData()
    {
        _workspaces.AddRange(new Workspace[] { new() { OwnerId = "1", Id = "1" }, new() { OwnerId = "2", Id = "2" } });

        _folders.AddRange(new Folder[]
        {
            new() { OwnerId = "1", Id = "1" }, new() { OwnerId = "1", Id = "2", ParentId = "1" },
            new() { OwnerId = "2", Id = "3" }
        });

        _notes.AddRange(new Note[]
        {
            new() { OwnerId = "1", FolderId = "1" }, new() { OwnerId = "1", FolderId = "1" },
            new() { OwnerId = "2" }
        });

        _templates.AddRange(new NoteTemplate[]
        {
            new() { OwnerId = "1" }, new() { OwnerId = "1" }, new() { OwnerId = "2" }
        });
    }
}
