using Moq;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Notes.Features.Folders;
using Notescrib.Notes.Features.Notes;
using Notescrib.Notes.Services;
using Notescrib.Notes.Tests.Infrastructure;
using Notescrib.Notes.Tests.Infrastructure.Extensions;

namespace Notescrib.Notes.Tests.Features.Folders.Commands;

using static Notescrib.Notes.Features.Folders.Commands.DeleteFolder;

public class DeleteFolderCommandHandlerTests
{
    private readonly Mock<IUserContextProvider> _userContextProviderMock = new();
    private readonly List<Folder> _folders = new();
    private readonly List<Note> _notes = new();

    private readonly Handler _sut;
    private readonly TestMongoDbContext _context = new();

    public DeleteFolderCommandHandlerTests()
    {
        _userContextProviderMock.SetupUserId("1");

        var rootFolder = new Folder
        {
            Id = "1",
            OwnerId = "1",
            Children = new List<Folder>
            {
                new()
                {
                    Id = "2",
                    ParentId = "1",
                    OwnerId = "1",
                    Children = new List<Folder> { new() { Id = "3", ParentId = "2", OwnerId = "1" } }
                }
            }
        };

        var unownedRootFolder = new Folder
        {
            Id = "-1",
            OwnerId = "2",
            Children = new List<Folder> { new() { Id = "-2", ParentId = "-1", OwnerId = "2" } }
        };

        _notes.AddRange(new Note[]
        {
            new() { Id = "1", OwnerId = "1", FolderId = "1", RelatedIds = { "3" } },
            new() { Id = "2", OwnerId = "1", FolderId = "2" }, new() { Id = "3", OwnerId = "1", FolderId = "3" },
            new() { Id = "-1", OwnerId = "1", FolderId = "-1" }
        });

        _folders.AddRange(new[]
        {
            rootFolder, rootFolder.Children.First(), rootFolder.Children.First().Children.First(),
            unownedRootFolder, unownedRootFolder.Children.First()
        });

        var mocks = _context.Mocks;

        mocks.Folders
            .SetupGetById(id => _folders.FirstOrDefault(x => x.Id == id))
            .SetupDeleteMany(ids => _folders.RemoveAll(x => ids.Contains(x.Id)));

        mocks.Notes
            .SetupGetIdsFromFolders(ids =>
                _notes.Where(x => ids.Contains(x.FolderId)).Select(x => x.Id).ToArray())
            .SetupDeleteFromRelated(ids =>
            {
                var relatedIds = ids.ToArray();
                var related = _notes.Where(x => x.RelatedIds.Intersect(relatedIds).Any());

                foreach (var note in related)
                {
                    note.RelatedIds = note.RelatedIds.Except(relatedIds).ToList();
                }
            })
            .SetupDeleteMany(ids => _notes.RemoveAll(x => ids.Contains(x.Id)));

        _sut = new(_context, new PermissionGuard(_userContextProviderMock.Object));
    }

    [Fact]
    public async Task Handle_WhenFolderExists_DeletesItWithAllChildrenAndNotes()
    {
        await _sut.Handle(new("2"), default);

        Assert.Collection(_folders,
            x => Assert.Equal("1", x.Id),
            x => Assert.Equal("-1", x.Id),
            x => Assert.Equal("-2", x.Id));
        
        Assert.Collection(_notes,
            x =>
            {
                Assert.True(x.Id == "1");
                Assert.Empty(x.RelatedIds);
            },
            x => Assert.True(x.Id == "-1"));
    }

    [Fact]
    public Task Handle_WhenFolderDoesNotExist_ThrowsNotFoundException()
        => Assert.ThrowsAsync<NotFoundException>(
            () => _sut.Handle(new("100"), default));
    
    [Fact]
    public Task Handle_WhenFolderIsRoot_ThrowsAppException()
        => Assert.ThrowsAsync<AppException>(
            () => _sut.Handle(new("1"), default));

    [Fact]
    public Task Handle_WhenUserCannotEditFolder_ThrowsForbiddenException()
        => Assert.ThrowsAsync<ForbiddenException>(
            () => _sut.Handle(new("-2"), default));
}
