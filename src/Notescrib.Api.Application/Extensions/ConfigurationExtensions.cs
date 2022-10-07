using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Notescrib.Api.Application.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection ConfigureSettings<TSettings>(this IServiceCollection services, IConfiguration config, string? sectionName = null)
        where TSettings : class
    {
        sectionName ??= typeof(TSettings).Name;

        services.Configure<TSettings>(config.GetSection(sectionName));

        return services;
    }
}
