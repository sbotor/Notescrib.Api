using Notescrib.Api.Core.Entities;
using Notescrib.Api.Infrastructure.Identity.Models;

namespace Notescrib.Api.Infrastructure.Identity.Mappers;

internal interface IUserDataMapper
{
    UserData MapToData(User item);
    User MapToEntity(UserData item);
}
