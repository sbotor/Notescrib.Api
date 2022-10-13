using Notescrib.Api.Application.Common.Services;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Enums;

namespace Notescrib.Api.Application.Tests.Services;

public class PermissionServiceTests
{
    private readonly PermissionService _sut;

    private readonly TestUserContextService _userContextService = new();

    public PermissionServiceTests()
    {
        _sut = new(_userContextService);
    }

    [Theory]
    [InlineData("123", "123", true)]
    [InlineData("123", "231", false)]
    [InlineData("123", null, false)]
    public void CanEdit_ReturnsTrueForOwnerOnly(string ownerId, string? userId, bool canEdit)
    {
        _userContextService.UserId = userId;

        var result = _sut.CanEdit(ownerId);

        Assert.Equal(canEdit, result);
    }

    [Theory]
    [InlineData("123", "123", true)]
    [InlineData("123", "231", false)]
    [InlineData("123", null, false)]
    public void CanView_ReturnsValid_WhenVisibilityPrivate(string userId, string ownerId, bool canView)
    {
        _userContextService.UserId = userId;
        var sharingDetails = new SharingDetails
        {
            Visibility = Visibility.Private
        };

        var result = _sut.CanView(ownerId, sharingDetails);

        Assert.Equal(canView, result);
    }

    [Theory]
    [InlineData("123", new[] { "123", "890" }, true)]
    [InlineData("123", new[] { "321", "890" }, false)]
    [InlineData("123", new string[0], false)]
    public void CanView_ReturnsValid_WhenVisibilityHidden(string userId, IEnumerable<string> allowedIds, bool canView)
    {
        _userContextService.UserId = userId;
        var sharingDetails = new SharingDetails
        {
            Visibility = Visibility.Hidden,
            AllowedUserIds = allowedIds.ToList()
        };

        var result = _sut.CanView("owner", sharingDetails);

        Assert.Equal(canView, result);
    }

    [Theory]
    [InlineData("123", "123")]
    [InlineData("123", "231")]
    [InlineData("123", null)]
    public void CanView_AllowsAll_WhenVisibilityPublic(string ownerId, string userId)
    {
        _userContextService.UserId = userId;
        var sharingDetails = new SharingDetails
        {
            Visibility = Visibility.Public,
        };

        var result = _sut.CanView(ownerId, sharingDetails);

        Assert.True(result);
    }
}
