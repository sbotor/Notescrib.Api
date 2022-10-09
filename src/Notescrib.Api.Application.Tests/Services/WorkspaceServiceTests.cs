using System.Net;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using Notescrib.Api.Application.Contracts.Workspace;
using Notescrib.Api.Application.Mappers;
using Notescrib.Api.Application.Repositories;
using Notescrib.Api.Application.Services;
using Notescrib.Api.Application.Services.Notes;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Enums;

namespace Notescrib.Api.Application.Tests.Services;

public class WorkspaceServiceTests
{
    private readonly WorkspaceService _sut;

    private readonly List<Workspace> _workspaces = new();
    private readonly TestUserContextService _userContextService = new();

    public WorkspaceServiceTests()
    {
        _userContextService.UserId = "123";
        _workspaces.Add(new Workspace
        {
            Name = "First workspace",
            Id = Guid.NewGuid().ToString(),
            OwnerId = _userContextService.UserId,
        });

        var workspaceRepoMock = new Mock<IWorkspaceRepository>();

        workspaceRepoMock
            .Setup(x => x.AddWorkspaceAsync(It.IsAny<Workspace>()))
            .ReturnsAsync(new Func<Workspace, Workspace>(w =>
            {
                w.Id = Guid.NewGuid().ToString();
                _workspaces.Add(w);
                return w;
            }));

        workspaceRepoMock
            .Setup(x => x.GetWorkspaceByIdAsync(It.IsAny<string>()))
            .ReturnsAsync(new Func<string, Workspace?>(w => _workspaces.FirstOrDefault(x => x.Id == w)));

        workspaceRepoMock
            .Setup(x => x.UpdateWorkspaceAsync(It.IsAny<Workspace>()))
            .Callback(new Action<Workspace>(w =>
            {
                var found = _workspaces.First(x => x.Id == w.Id);
                _workspaces.Remove(found);
                _workspaces.Add(w);
            }));

        _sut = new WorkspaceService(
            workspaceRepoMock.Object,
            new PermissionService(_userContextService),
            _userContextService,
            new WorkspaceMapper(),
            NullLogger<WorkspaceService>.Instance);
    }

    [Fact]
    public async Task AddWorkspace_Successful()
    {
        var workspaceName = "Test workspace";
        var request = new WorkspaceRequest
        {
            Name = workspaceName,
            SharingDetails = new()
            {
                Visibility = Core.Enums.Visibility.Hidden,
                AllowedUserIds = new List<string> { "321", "890" }
            }
        };

        var result = await _sut.AddWorkspaceAsync(request);
        var response = result.Response;

        Assert.True(result.IsSuccessful);
        Assert.NotNull(response);
        Assert.Equal(workspaceName, response?.Name);
        Assert.Equal(_userContextService.UserId, response?.OwnerId);

        var sharingDetails = response?.SharingDetails;
        Assert.NotNull(sharingDetails);
        Assert.Equal(request.SharingDetails.Visibility, sharingDetails?.Visibility);
        Assert.Equal(request.SharingDetails.AllowedUserIds, sharingDetails?.AllowedUserIds);
    }

    [Fact]
    public async Task GetWorkspaceById_ReturnsWorkspace_WhenExists()
    {
        var workspaceId = _workspaces.First().Id!;

        var result = await _sut.GetWorkspaceByIdAsync(workspaceId);
        var response = result.Response;

        Assert.True(result.IsSuccessful);
        Assert.NotNull(response);
    }

    [Fact]
    public async Task GetWorkspaceById_Fails_WhenDoesNotExist()
    {
        var workspaceId = _workspaces.First().Id! + "-TEST";

        var result = await _sut.GetWorkspaceByIdAsync(workspaceId);
        var response = result.Response;

        Assert.False(result.IsSuccessful);
        Assert.Null(response);
        Assert.Equal(HttpStatusCode.NotFound, result.StatusCode);
    }

    [Theory]
    [InlineData("123", Visibility.Public, true)]
    [InlineData("123", Visibility.Hidden, true)]
    [InlineData("123", Visibility.Private, true)]
    [InlineData("321", Visibility.Public, true)]
    [InlineData("321", Visibility.Hidden, false)]
    [InlineData("321", Visibility.Private, false)]
    public async Task GetWorkspaceById_RespondsCorrectly_ForDifferentPermissions(string ownerId, Visibility visibility, bool shouldAccess)
    {
        var workspace = new Workspace
        {
            Id = Guid.NewGuid().ToString(),
            Name = "Test workspace",
            OwnerId = ownerId,
            SharingDetails = new SharingDetails
            {
                Visibility = visibility
            }
        };
        _workspaces.Add(workspace);

        var result = await _sut.GetWorkspaceByIdAsync(workspace.Id);
        var response = result.Response;

        Assert.Equal(shouldAccess, result.IsSuccessful);
        Assert.True(shouldAccess ? result.StatusCode == HttpStatusCode.OK : result.StatusCode == HttpStatusCode.Forbidden);
        Assert.True(shouldAccess ? response != null : response == null);
    }

    [Fact]
    public async Task AddFolder_Fails_WhenFolderExists()
    {
        var folderName = "Test folder";
        var workspaceId = Guid.NewGuid().ToString();

        var request = new FolderRequest
        {
            Name = folderName,
            ParentPath = workspaceId
        };

        _workspaces.Add(new Workspace
        {
            Id = workspaceId,
            OwnerId = _userContextService.UserId!,
            Name = "Test workspace",
            Folders = new List<FolderPath>
            {
                new FolderPath
                {
                    Name = folderName,
                    ParentPath = workspaceId
                }
            }
        });

        var result = await _sut.AddFolderAsync(request);

        Assert.False(result.IsSuccessful);
        Assert.Equal(HttpStatusCode.BadRequest, result.StatusCode);
    }
}
