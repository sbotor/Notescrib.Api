using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Notescrib.Core.Api.Extensions;

public static class SecurityExtensions
{
    public static IServiceCollection ConfigureJwtAuth(this IServiceCollection services,
        Action<TokenValidationParameters>? configurationAction = null)
    {
        var validationParams = new TokenValidationParameters();
        configurationAction?.Invoke(validationParams);

        services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = validationParams;
        });

        return services;
    }
}
