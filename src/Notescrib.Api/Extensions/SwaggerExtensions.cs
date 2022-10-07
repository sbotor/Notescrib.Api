using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace Notescrib.Api.Extensions;

public static class SwaggerExtensions
{
    private const string JwtSchemeName = "JWT";

    public static IServiceCollection ConfigureSwagger(this IServiceCollection services)
    {
        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition(JwtSchemeName, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.ApiKey,
                In = ParameterLocation.Header,
                Name = HeaderNames.Authorization,
                Description = "JWT Bearer scheme.",
                Scheme = "Bearer"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = JwtSchemeName
                        }
                    },
                    Array.Empty<string>()
                }
            });
        });

        return services;
    }
}
