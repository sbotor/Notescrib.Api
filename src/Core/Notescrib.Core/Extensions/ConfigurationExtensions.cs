using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Notescrib.Core.Extensions;

public static class ConfigurationExtensions
{
    public static IServiceCollection ConfigureSettings<TSettings>(this IServiceCollection services,
        IConfiguration config,
        string? name = null)
        where TSettings : class
    {
        name ??= typeof(TSettings).Name;
        services.Configure<TSettings>(config.GetSection(name));

        return services;
    }

    public static TSettings? GetSettings<TSettings>(this IConfiguration config, string? name = null)
        where TSettings : class
    {
        name ??= typeof(TSettings).Name;
        return config.GetSection(name).Get<TSettings>();
    }
}
