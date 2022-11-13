using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;

namespace Notescrib.Core.Api.Extensions;

public static class SecurityExtensions
{
    public static AuthenticationBuilder ConfigureJwtAuth(this IServiceCollection services,
        Action<TokenValidationParameters>? configurationAction = null)
    {
        var validationParams = new TokenValidationParameters();
        configurationAction?.Invoke(validationParams);

        return services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(options =>
        {
            options.TokenValidationParameters = validationParams;
        });
    }
}
