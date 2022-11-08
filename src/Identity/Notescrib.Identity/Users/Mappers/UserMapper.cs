using Notescrib.Identity.Common.Entities;
using Notescrib.Identity.Users.Commands;
using Notescrib.Identity.Users.Models;

namespace Notescrib.Identity.Users.Mappers;

internal class UserMapper : IUserMapper
{
    public AppUser MapToEntity(CreateUser.Command item)
        => new()
        {
            UserName = item.Email,
            Email = item.Email,
            IsActive = true,
            EmailConfirmed = true // TODO: Email confirmation.
        };

    public UserDetails MapToDetails(AppUser item)
        => new() { Email = item.Email, IsActive = item.IsActive, Id = item.Id };
}
