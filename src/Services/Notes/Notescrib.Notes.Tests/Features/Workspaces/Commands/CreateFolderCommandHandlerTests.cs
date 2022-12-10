using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Folders;
using Notescrib.Notes.Features.Folders.Mappers;
using Notescrib.Notes.Services;
using Notescrib.Notes.Tests.Infrastructure;
using Notescrib.Notes.Utils;
using Notescrib.Notes.Utils.Tree;
using static Notescrib.Notes.Features.Folders.Commands.CreateFolder;

namespace Notescrib.Notes.Tests.Features.Workspaces.Commands;

public class CreateFolderWorkspaceCommandHandlerTests
{
    private readonly TestUserContextProvider _userContext = new();
    private readonly TestWorkspaceRepository _repository = new();

    private readonly Handler _sut;

    public CreateFolderWorkspaceCommandHandlerTests()
    {
        _sut = new(_repository, new PermissionGuard(_userContext), new FolderOverviewMapper(),
            new UtcDateTimeProvider());

        _userContext.UserId = "1";

        WorkspaceDataSetup.SetupWorkspace(_repository);
    }

    [Fact]
    public Task Handle_WhenWorkspaceDoesNotExist_ThrowsNotFoundException()
        => Assert.ThrowsAnyAsync<NotFoundException>(
            () => _sut.Handle(new("Folder 2", Folder.RootId), default));

    [Fact]
    public Task Handle_WhenParentFolderDoesNotExist_ThrowsNotFoundException()
        => Assert.ThrowsAnyAsync<NotFoundException>(
            () => _sut.Handle(new("Folder 2.0", "F2"), default));

    [Fact]
    public async Task Handle_WhenUserCannotEditWorkspace_ThrowsForbiddenException()
    {
        _userContext.UserId = "asdf";

        await Assert.ThrowsAnyAsync<ForbiddenException>(
            () => _sut.Handle(new("Folder 2", Folder.RootId), default));
    }

    [Fact]
    public async Task Handle_WhenNestingIsTooDeep_ThrowsAppException()
    {
        var folder = new Folder { Id = "N0", Name = "Nested 0" };
        var tempFolder = folder;

        for (var i = 1; i <= Consts.Folder.MaxNestingLevel; i++)
        {
            var newFolder = new Folder { Id = "N{i}", Name = $"Nested {i}" };
            tempFolder.Children = new List<Folder> { newFolder };
            tempFolder = newFolder;
        }

        _repository.Items.First().FolderTree = Folder.CreateRoot(null, folder);

        await Assert.ThrowsAnyAsync<AppException>(
            () => _sut.Handle(new(
                    $"Nested {Consts.Folder.MaxNestingLevel + 1}", $"N{Consts.Folder.MaxNestingLevel}"),
                default));
    }

    [Theory]
    [InlineData("Folder 2", null)]
    [InlineData("Folder 1.0", "F1")]
    [InlineData("Folder 0.0.0", "F0.0")]
    public async Task Handle_ForCorrectCircumstances_CreatesFolder(string name, string? parentId)
    {
        await _sut.Handle(new(name, parentId ?? Folder.RootId), default);

        var tree = new Tree<Folder>(_repository.Items.First().FolderTree);

        Assert.Single(tree.EnumerateBreadthFirst(), x => (parentId == null && x.Item.Name == name)
                                 || (x.Item.Id == parentId && x.Item.Children.SingleOrDefault(c => c.Name == name) != null));
    }
}
