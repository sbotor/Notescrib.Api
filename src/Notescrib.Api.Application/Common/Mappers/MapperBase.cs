using AutoMapper;

namespace Notescrib.Api.Application.Common.Mappers;

internal abstract class MapperBase : Profile, IMapperBase
{
    protected IMapper InternalMapper { get; }

    public MapperBase()
    {
        ConfigureMappings();

        InternalMapper = new Mapper(new MapperConfiguration(cfg => cfg.AddProfile(this)));
    }

    protected abstract void ConfigureMappings();

    public TDestination Map<TSource, TDestination>(TSource source)
        => InternalMapper.Map<TSource, TDestination>(source);
    
    public TDestination Map<TDestination>(object source)
        => InternalMapper.Map<TDestination>(source);
    
    public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        => InternalMapper.Map(source, destination);
    
    public object Map(object source, Type sourceType, Type destinationType)
        => InternalMapper.Map(source, sourceType, destinationType);
    
    public object Map(object source, object destination, Type sourceType, Type destinationType)
        => InternalMapper.Map(source, destination, sourceType, destinationType);
}
