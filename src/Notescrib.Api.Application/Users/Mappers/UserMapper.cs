using Notescrib.Api.Application.Common.Mappers;
using Notescrib.Api.Application.Users.Models;
using Notescrib.Api.Core.Entities;

namespace Notescrib.Api.Application.Users.Mappers;

internal class UserMapper : MapperBase, IUserMapper
{
    protected override void ConfigureMappings()
    {
        CreateMap<User, UserDetails>();
    }
}
