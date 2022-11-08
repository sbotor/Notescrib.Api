using Microsoft.Net.Http.Headers;
using Microsoft.OpenApi.Models;

namespace Notescrib.Notes.Api.Extensions;

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
                Type = SecuritySchemeType.Http,
                BearerFormat = "JWT",
                In = ParameterLocation.Header,
                Name = HeaderNames.Authorization,
                Scheme = "Bearer",
                Description = "JWT Bearer scheme.",
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
