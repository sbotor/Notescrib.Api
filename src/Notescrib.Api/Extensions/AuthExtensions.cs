using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using Notescrib.Api.Application.Common.Configuration;

namespace Notescrib.Api.Extensions;

public static class AuthExtensions
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
}
