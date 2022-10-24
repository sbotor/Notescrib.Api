using AutoMapper;
using Notescrib.Api.Core.Entities;
using Notescrib.Api.Infrastructure.Identity.Models;

namespace Notescrib.Api.Infrastructure;

internal class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<User, UserData>()
            .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

        CreateMap<UserData, User>();
    }
}
