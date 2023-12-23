using System.Text;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Notescrib.Core.Api.Configuration;

namespace Notescrib.Core.Api.Extensions;

public static class SecurityExtensions
{
    public static AuthenticationBuilder ConfigureJwtAuth(this IServiceCollection services, JwtSettings settings)
        => services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new()
            {
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(settings.Key)),
                ValidateIssuerSigningKey = true,
                ValidateIssuer = true,
                ValidIssuer = settings.Issuer,
                ValidateAudience = true,
                ValidAudience = settings.Audience,
                ValidateLifetime = true,
            };
        });

    public static bool TryAddCors(this IServiceCollection services, IConfiguration config)
    {
        var allowedOrigins = config.GetSection("AllowedOrigins")
            .Get<string[]>();

        if (allowedOrigins is null)
        {
            return false;
        }

        services.AddCors(x =>
        {
            x.AddDefaultPolicy(y =>
            {
                y.AllowAnyHeader()
                    .AllowAnyMethod()
                    .WithOrigins(allowedOrigins);
            });
        });

        return true;
    }
}
