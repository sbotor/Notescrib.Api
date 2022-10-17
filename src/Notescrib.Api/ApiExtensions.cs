using System.Text.Json.Serialization;
using Notescrib.Api.Application;
using Notescrib.Api.Extensions;
using Notescrib.Api.Infrastructure;

namespace Notescrib.Api;

public static class ApiExtensions
{
    public static WebApplicationBuilder Configure(this WebApplicationBuilder builder)
    {
        builder.Services.AddControllers()
            .ConfigureSerialization();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.ConfigureSwagger();
        }

        builder.Services
            .AddApplicationServices(builder.Configuration)
            .AddInfrastructure(builder.Configuration);

        builder.Services.ConfigureAuthentication(builder.Configuration);

        return builder;
    }

    public static IMvcBuilder ConfigureSerialization(this IMvcBuilder builder)
    {
        builder.AddJsonOptions(options =>
        {
            var converters = options.JsonSerializerOptions.Converters;

            converters.Add(new JsonStringEnumConverter());
        });

        return builder;
    }
}
