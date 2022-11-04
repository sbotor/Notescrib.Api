using Notescrib.Api.Application.Common;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Core.Enums;

namespace Notescrib.Api.Application.Tests.Common.Services;

public class SharingServiceTests
{
    private readonly SharingService _sut;

    private readonly TestUserContextProvider _userContext = new();

    public SharingServiceTests()
    {
        _sut = new(_userContext);
    }

    [Theory]
    [InlineData("123", "123", true)]
    [InlineData("123", "231", false)]
    [InlineData("123", null, false)]
    public void CanEdit_ReturnsTrueForOwnerOnly(string ownerId, string? userId, bool canEdit)
    {
        _userContext.UserId = userId;

        var result = _sut.CanEdit(ownerId);

        Assert.Equal(canEdit, result);
    }

    [Theory]
    [InlineData("123", "123", true)]
    [InlineData("123", "231", false)]
    [InlineData("123", null, false)]
    public void CanView_ReturnsValid_WhenVisibilityPrivate(string userId, string ownerId, bool canView)
    {
        _userContext.UserId = userId;
        var sharingInfo = new SharingInfo
        {
            Visibility = VisibilityLevel.Private
        };

        var result = _sut.CanView(ownerId, sharingInfo);

        Assert.Equal(canView, result);
    }

    [Theory]
    [InlineData("123", new[] { "123", "890" }, true)]
    [InlineData("123", new[] { "321", "890" }, false)]
    [InlineData("123", new string[0], false)]
    public void CanView_ReturnsValid_WhenVisibilityHidden(string userId, IEnumerable<string> allowedIds, bool canView)
    {
        _userContext.UserId = userId;
        var sharingInfo = new SharingInfo
        {
            Visibility = VisibilityLevel.Hidden,
            AllowedUserIds = allowedIds.ToList()
        };

        var result = _sut.CanView("owner", sharingInfo);

        Assert.Equal(canView, result);
    }

    [Theory]
    [InlineData("123", "123")]
    [InlineData("123", "231")]
    [InlineData("123", null)]
    public void CanView_AllowsAll_WhenVisibilityPublic(string ownerId, string userId)
    {
        _userContext.UserId = userId;
        var sharingInfo = new SharingInfo
        {
            Visibility = VisibilityLevel.Public,
        };

        var result = _sut.CanView(ownerId, sharingInfo);

        Assert.True(result);
    }
}
