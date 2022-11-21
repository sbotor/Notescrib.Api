using Notescrib.Identity.Features.Users.Commands;
using Notescrib.Identity.Features.Users.Models;

namespace Notescrib.Identity.Features.Users.Mappers;

internal interface IUserMapper
{
    UserDetails MapToDetails(AppUser item);
    AppUser MapToEntity(CreateUser.Command item);
}
