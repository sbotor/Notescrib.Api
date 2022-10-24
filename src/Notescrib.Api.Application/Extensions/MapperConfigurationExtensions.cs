using System.Linq.Expressions;
using AutoMapper;

namespace Notescrib.Api.Application.Extensions;

internal static class MapperConfigurationExtensions
{
    public static IMappingExpression<TSource, TDestination> Ignore<TSource, TDestination, TProperty>(
        this IMappingExpression<TSource, TDestination> expression,
        Expression<Func<TDestination, TProperty>> destinationMember)
        => expression.ForMember(destinationMember, opt => opt.Ignore());
}
