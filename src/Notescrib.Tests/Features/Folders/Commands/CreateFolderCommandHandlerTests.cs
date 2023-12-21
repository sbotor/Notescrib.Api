using Moq;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Features.Folders;
using Notescrib.Services;
using Notescrib.Tests.Infrastructure;
using Notescrib.Tests.Infrastructure.Extensions;
using Notescrib.Utils;
using static Notescrib.Features.Folders.Commands.CreateFolder;

namespace Notescrib.Tests.Features.Folders.Commands;

public class CreateFolderCommandHandlerTests
{
    private readonly Mock<IUserContextProvider> _userContextProviderMock = new();
    private readonly List<Folder> _folders = new();

    private readonly Handler _sut;
    private readonly TestMongoDbContext _context = new();

    public CreateFolderCommandHandlerTests()
    {
        _userContextProviderMock.SetupUserId("1");

        var rootFolder = new Folder
        {
            Id = "1",
            Name = Folder.RootName,
            OwnerId = "1",
            Children = new List<Folder>
            {
                new()
                {
                    Id = "2",
                    Name = "folder",
                    ParentId = "1",
                    OwnerId = "1",
                    AncestorIds = { "1" }
                }
            }
        };

        var unownedRootFolder = new Folder
        {
            Id = "-1",
            Name = Folder.RootName,
            OwnerId = "2",
            Children = new List<Folder>
            {
                new()
                {
                    Id = "-2",
                    Name = "folder",
                    ParentId = "-1",
                    OwnerId = "2",
                    AncestorIds = { "-1" }
                }
            }
        };

        _folders.AddRange(new[]
        {
            rootFolder, rootFolder.Children.First(), unownedRootFolder, unownedRootFolder.Children.First()
        });

        var mocks = _context.Mocks;

        mocks.Folders
            .SetupGetRoot(() =>
                _folders.FirstOrDefault(x => x.ParentId == null && x.OwnerId == _userContextProviderMock.Object.UserId))
            .SetupGetById(id => _folders.FirstOrDefault(x => x.Id == id));

        _sut = new(_context, new PermissionGuard(_userContextProviderMock.Object), new UtcDateTimeProvider());
    }

    [Theory]
    [InlineData(null)]
    [InlineData("2")]
    public async Task Handle_WhenParentExists_AddsNewChild(string? parentId)
    {
        var parent = parentId != null ? _folders.Skip(1).First() : _folders.First();
        _context.Mocks.Folders.SetupCreate(x => parent.Children.Add(x));

        await _sut.Handle(new("test", parentId), default);

        var created = parent.Children.Single(x => x.Name == "test");
        Assert.NotNull(created);
        Assert.True(created.ParentId == parent.Id);
        Assert.True(created.Name == "test");
        Assert.True(created.OwnerId == "1");
    }

    [Fact]
    public Task Handle_WhenParentDoesNotExist_ThrowsNotFoundException()
        => Assert.ThrowsAsync<NotFoundException>(
            () => _sut.Handle(new("test", "100"), default));

    [Fact]
    public Task Handle_WhenUserCannotEditParent_ThrowsForbiddenException()
        => Assert.ThrowsAsync<ForbiddenException>(
            () => _sut.Handle(new("test", "3"), default));

    [Fact]
    public Task Handle_WhenMaxChildrenCountReached_ThrowsAppException()
    {
        var parent = _folders.First();
        var newChildren = new List<Folder>();

        for (var i = parent.Children.Count; i <= Consts.Folder.MaxChildrenCount; i++)
        {
            newChildren.Add(new() { ParentId = parent.Id, Id = $"{100 + i}" });
        }

        parent.Children = parent.Children.Concat(newChildren).ToArray();

        return Assert.ThrowsAsync<AppException>(
            () => _sut.Handle(new("test", null), default));
    }

    [Fact]
    public Task Handle_WhenMaxNestingLevelReached_ThrowsAppException()
    {
        var parent = _folders.Skip(1).First();

        while (parent.AncestorIds.Count <= Consts.Folder.MaxNestingLevel)
        {
            parent.Children.Add(new()
            {
                AncestorIds = parent.AncestorIds.Append(parent.Id).ToArray(),
                ParentId = parent.Id,
                Id = $"{10 * parent.AncestorIds.Count}",
                OwnerId = "1"
            });

            parent = parent.Children.First();
            _folders.Add(parent);
        }

        return Assert.ThrowsAsync<AppException>(
            () => _sut.Handle(new("test", parent.Id), default));
    }

    [Fact]
    public Task Handle_WhenFolderAlreadyExists_ThrowsDuplicationException()
    {
        var root = _folders.First();
        var existing = new Folder { Id = "20", Name = "test", ParentId = root.Id };

        root.Children.Add(existing);
        _folders.Add(existing);

        return Assert.ThrowsAsync<DuplicationException>(
            () => _sut.Handle(new("test", null), default));
    }
}
