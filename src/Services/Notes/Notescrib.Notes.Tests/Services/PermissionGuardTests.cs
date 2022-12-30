using Moq;
using Notescrib.Core.Models.Exceptions;
using Notescrib.Core.Services;
using Notescrib.Notes.Models.Enums;
using Notescrib.Notes.Services;
using Notescrib.Notes.Tests.Infrastructure.Extensions;

namespace Notescrib.Notes.Tests.Services;

public class PermissionGuardTests
{
    private readonly Mock<IUserContextProvider> _userContextProviderMock = new();
    private readonly PermissionGuard _sut;

    public PermissionGuardTests()
    {
        _userContextProviderMock.SetupUserId("1");

        _sut = new(_userContextProviderMock.Object);
    }

    [Fact]
    public void CanEdit_WhenUserIsTheOwner_ReturnsTrue()
        => Assert.True(_sut.CanEdit("1"));

    [Theory]
    [InlineData("100")]
    [InlineData(null)]
    public void CanEdit_WhenUserIsNotTheOwner_ReturnsFalse(string? userId)
    {
        _userContextProviderMock.SetupUserId(userId);
        Assert.False(_sut.CanEdit("1"));
    }

    [Fact]
    public void CanView_ForNoVisibilityWhenUserIsTheOwner_ReturnsTrue()
        => Assert.True(_sut.CanView("1"));

    [Theory]
    [InlineData("100")]
    [InlineData(null)]
    public void CanView_ForNoVisibilityWhenUserIsNotTheOwner_ReturnsFalse(string? userId)
    {
        _userContextProviderMock.SetupUserId(userId);
        Assert.False(_sut.CanView("1"));
    }

    [Fact]
    public void CanView_ForPrivateVisibilityWhenUserIsTheOwner_ReturnsTrue()
        => Assert.True(_sut.CanView("1", new() { Visibility = VisibilityLevel.Private }));

    [Theory]
    [InlineData("100")]
    [InlineData(null)]
    public void CanView_ForPrivateVisibilityWhenUserIsNotTheOwner_ReturnsFalse(string? userId)
    {
        _userContextProviderMock.SetupUserId(userId);
        Assert.False(_sut.CanView("1", new() { Visibility = VisibilityLevel.Private }));
    }

    [Theory]
    [InlineData("1", VisibilityLevel.Hidden)]
    [InlineData("100", VisibilityLevel.Hidden)]
    [InlineData(null, VisibilityLevel.Hidden)]
    [InlineData("1", VisibilityLevel.Public)]
    [InlineData("100", VisibilityLevel.Public)]
    [InlineData(null, VisibilityLevel.Public)]
    public void CanView_ForNonPrivateVisibilityAlways_ReturnsTrue(string? userId, VisibilityLevel visibility)
    {
        _userContextProviderMock.SetupUserId(userId);
        Assert.True(_sut.CanView("1", new() { Visibility = visibility }));
    }
    
    [Fact]
    public void GuardCanEdit_WhenUserIsTheOwner_DoesNotThrow()
        => _sut.GuardCanEdit("1");

    [Theory]
    [InlineData("100")]
    [InlineData(null)]
    public void GuardCanEdit_WhenUserIsNotTheOwner_ThrowsForbiddenException(string? userId)
    {
        _userContextProviderMock.SetupUserId(userId);
        Assert.Throws<ForbiddenException>(() => _sut.GuardCanEdit("1"));
    }
    
    [Fact]
    public void GuardCanView_ForNoVisibilityWhenUserIsTheOwner_DoesNotThrow()
        => _sut.GuardCanView("1");

    [Theory]
    [InlineData("100")]
    [InlineData(null)]
    public void GuardCanView_ForNoVisibilityWhenUserIsNotTheOwner_ThrowsForbiddenException(string? userId)
    {
        _userContextProviderMock.SetupUserId(userId);
        Assert.Throws<ForbiddenException>(() => _sut.GuardCanView("1"));
    }
    
    [Fact]
    public void GuardCanView_ForPrivateVisibilityWhenUserIsTheOwner_DoesNotThrow()
        => _sut.GuardCanView("1", new() { Visibility = VisibilityLevel.Private });

    [Theory]
    [InlineData("100")]
    [InlineData(null)]
    public void GuardCanView_ForPrivateVisibilityWhenUserIsNotTheOwner_ThrowsForbiddenException(string? userId)
    {
        _userContextProviderMock.SetupUserId(userId);
        Assert.Throws<ForbiddenException>(() => _sut.GuardCanView("1", new() { Visibility = VisibilityLevel.Private }));
    }
}
