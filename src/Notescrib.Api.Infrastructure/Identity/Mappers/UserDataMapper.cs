using Notescrib.Api.Core.Entities;
using Notescrib.Api.Infrastructure.Identity.Models;

namespace Notescrib.Api.Infrastructure.Identity.Mappers;

internal class UserDataMapper : IUserDataMapper
{
    public UserData MapToData(User item)
        => new()
        {
            UserName = item.Email,
            Email = item.Email,
            IsActive = item.IsActive,
            EmailConfirmed = true // TODO: Email confirmation.
        };

    public User MapToEntity(UserData item)
        => new() { Email = item.Email, IsActive = item.IsActive, Id = item.Id };
}
