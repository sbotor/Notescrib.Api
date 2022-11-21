using System.Reflection;
using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Notescrib.Core.Cqrs.Behaviors;

namespace Notescrib.Core.Extensions;

public static class ServicesExtensions
{
    public static IServiceCollection AddMediatrWithValidation(this IServiceCollection services, params Assembly[] assemblies)
    {
        services.AddMediatR(assemblies);
        services.AddValidatorsFromAssemblies(assemblies, includeInternalTypes: true);
        services.AddPipelineBehavior(typeof(ValidationBehavior<,>));
        
        return services;
    }
    
    public static IServiceCollection AddPipelineBehavior(this IServiceCollection services, Type behaviorType)
        => services.AddScoped(typeof(IPipelineBehavior<,>), behaviorType);
}
