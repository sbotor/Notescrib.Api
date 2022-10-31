using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Notescrib.Api.Application.Common.Configuration;

namespace Notescrib.Api.Extensions;

public static class SecurityExtensions
{
    public static IServiceCollection ConfigureAuthentication(this IServiceCollection services, IConfiguration config)
    {
        var settings = config.GetSection(nameof(JwtSettings)).Get<JwtSettings>();
        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key));

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                IssuerSigningKey = key,
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = settings.Issuer,
                ValidateLifetime = true,
                ValidateAudience = false,
            };
        });

        return services;
    }

    public static IServiceCollection ConfigureCors(this IServiceCollection services, IConfiguration config)
    {
        var origins = config.GetSection("AllowedOrigins").Get<string[]>();

        services.AddCors(options =>
        {
            options.AddDefaultPolicy(x =>
                x.WithOrigins(origins)
                    .WithMethods("GET", "POST", "PUT", "DELETE", "OPTIONS")
                    .AllowAnyHeader());
        });

        return services;
    }
}
