using System.Text.Json.Serialization;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Notescrib.Core.Api.Setup;

namespace Notescrib.Core.Api.Extensions;

public static class WebApplicationExtensions
{
    public static IMvcBuilder ConfigureSerialization(this IMvcBuilder builder)
        => builder.AddJsonOptions(options =>
        {
            var conv = options.JsonSerializerOptions.Converters;
            conv.Add(new JsonStringEnumConverter());
        });

    public static WebApplication UseModule<TModule>(this WebApplicationBuilder builder)
        where TModule : IModule, new()
    {
        var module = new TModule();
        module.AddServices(builder);

        var app = builder.Build();
        module.ConfigureApplication(app);
        
        return app;
    }
}
