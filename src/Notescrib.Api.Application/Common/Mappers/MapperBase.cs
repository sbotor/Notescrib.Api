using AutoMapper;

namespace Notescrib.Api.Application.Common.Mappers;

internal abstract class MapperBase : Profile
{
    public IMapper InternalMapper { get; }

    public MapperBase()
    {
        ConfigureMappings();

        InternalMapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(this)));
    }

    protected virtual void ConfigureMappings()
    {
    }
}
