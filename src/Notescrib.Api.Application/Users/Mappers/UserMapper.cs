using Notescrib.Api.Application.Users.Commands;
using Notescrib.Api.Application.Users.Models;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Users.Mappers;

public class UserMapper : IUserMapper
{
    public UserDetails MapToDetails(User item)
        => new() { Id = item.Id, Email = item.Email, IsActive = item.IsActive };

    public User MapToEntity(CreateUser.Command item)
        => new() { Email = item.Email, IsActive = true };
}
