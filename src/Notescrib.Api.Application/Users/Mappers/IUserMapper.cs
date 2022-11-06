using Notescrib.Api.Application.Users.Commands;
using Notescrib.Api.Application.Users.Models;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Users.Mappers;

public interface IUserMapper
{
    UserDetails MapToDetails(User item);
    User MapToEntity(CreateUser.Command item);
}
