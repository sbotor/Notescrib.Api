﻿using Notescrib.Notes.Models;
using Notescrib.Notes.Models.Enums;
using Notescrib.Notes.Models.Exceptions;

namespace Notescrib.Notes.Services;

internal class SharingGuard : ISharingGuard
{
    private readonly IUserContextProvider _userContext;

    public SharingGuard(IUserContextProvider userContext)
    {
        _userContext = userContext;
    }

    public bool CanEdit(string ownerId) => _userContext.UserId == ownerId;

    public void GuardCanEdit(string ownerId)
    {
        if (!CanEdit(ownerId))
        {
            throw new ForbiddenException();
        }
    }

    public bool CanView(string ownerId, SharingInfo sharingInfo)
    {
        var userId = _userContext.UserId;
        if (sharingInfo.Visibility == VisibilityLevel.Public
            || userId == ownerId)
        {
            return true;
        }

        if (sharingInfo.Visibility == VisibilityLevel.Hidden)
        {
            return userId != null
                && sharingInfo.AllowedIds.Contains(userId);
        }

        return false;
    }

    public void GuardCanView(string ownerId, SharingInfo sharingInfo)
    {
        if (!CanView(ownerId, sharingInfo))
        {
            throw new ForbiddenException();
        }
    }
}
