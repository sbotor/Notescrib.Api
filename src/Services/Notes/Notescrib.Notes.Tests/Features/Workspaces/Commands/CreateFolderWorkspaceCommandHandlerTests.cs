using Notescrib.Core.Models.Exceptions;
using Notescrib.Notes.Features.Workspaces;
using Notescrib.Notes.Features.Workspaces.Mappers;
using Notescrib.Notes.Features.Workspaces.Utils;
using Notescrib.Notes.Services;
using Notescrib.Notes.Tests.Infrastructure;
using Notescrib.Notes.Utils;
using static Notescrib.Notes.Features.Workspaces.Commands.CreateFolder;

namespace Notescrib.Notes.Tests.Features.Workspaces.Commands;

public class CreateFolderWorkspaceCommandHandlerTests
{
    private readonly TestUserContextProvider _userContext = new();
    private readonly TestWorkspaceRepository _repository = new();

    private readonly Handler _sut;

    public CreateFolderWorkspaceCommandHandlerTests()
    {
        _sut = new(_repository, new PermissionGuard(_userContext), new WorkspaceDetailsMapper());

        _userContext.UserId = "1";

        WorkspaceDataSetup.SetupWorkspace(_repository);
    }

    [Fact]
    public Task Handle_WhenWorkspaceDoesNotExist_ThrowsNotFoundException()
        => Assert.ThrowsAnyAsync<NotFoundException>(
            () => _sut.Handle(new("asdf", "Folder 2", null), default));

    [Fact]
    public Task Handle_WhenParentFolderDoesNotExist_ThrowsNotFoundException()
        => Assert.ThrowsAnyAsync<NotFoundException>(
            () => _sut.Handle(new("1", "Folder 2.0", "F2"), default));

    [Fact]
    public Task Handle_WhenFolderAlreadyExist_ThrowsDuplicationException()
        => Assert.ThrowsAnyAsync<DuplicationException>(
            () => _sut.Handle(new("1", "Folder 0", null), default));

    [Fact]
    public async Task Handle_WhenUserCannotEditWorkspace_ThrowsForbiddenException()
    {
        _userContext.UserId = "asdf";

        await Assert.ThrowsAnyAsync<ForbiddenException>(
            () => _sut.Handle(new("1", "Folder 2", null), default));
    }

    [Fact]
    public async Task Handle_WhenNestingIsTooDeep_ThrowsAppException()
    {
        var folder = new Folder { Id = "N0", Name = "Nested 0" };
        var tempFolder = folder;

        for (var i = 1; i <= Size.NestingLevel.Max; i++)
        {
            var newFolder = new Folder { Id = "N{i}", Name = $"Nested {i}" };
            tempFolder.Children = new List<Folder> { newFolder };
            tempFolder = newFolder;
        }

        _repository.Items.First().Folders = new[] { folder };

        await Assert.ThrowsAnyAsync<AppException>(
            () => _sut.Handle(new(
                    "1",
                    $"Nested {Size.NestingLevel.Max + 1}",
                    $"N{Size.NestingLevel.Max}"),
                default));
    }

    [Theory]
    [InlineData("Folder 2", null)]
    [InlineData("Folder 1.0", "Folder 1")]
    [InlineData("Folder 0.0.0", "Folder 0.0")]
    public async Task Handle_ForCorrectCircumstances_CreatesFolder(string name, string? parent)
    {
        await _sut.Handle(new("1", name, parent), default);

        var tree = new FolderTree(_repository.Items.First().Folders);

        Assert.Single(tree, x => (parent == null && x.Name == name)
            || (x.Name == parent&& x.Children.SingleOrDefault(c => c.Name == name) != null));
    }
}
