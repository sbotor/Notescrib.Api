using Notescrib.Identity.Common.Entities;
using Notescrib.Identity.Users.Commands;
using Notescrib.Identity.Users.Models;

namespace Notescrib.Identity.Users.Mappers;

internal interface IUserMapper
{
    UserDetails MapToDetails(AppUser item);
    AppUser MapToEntity(CreateUser.Command item);
}
