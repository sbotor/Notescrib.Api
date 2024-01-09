using System.Text.Json.Serialization;
using Microsoft.Extensions.DependencyInjection;

namespace Notescrib.Core.Api.Extensions;

public static class WebApplicationExtensions
{
    public static IMvcBuilder ConfigureSerialization(this IMvcBuilder builder)
        => builder.AddJsonOptions(options =>
        {
            var conv = options.JsonSerializerOptions.Converters;
            conv.Add(new JsonStringEnumConverter());
        });
}
