using System.Text;
using Microsoft.IdentityModel.Tokens;
using Notescrib.Core.Api.Extensions;
using Notescrib.Core.Api.Middleware;
using Notescrib.Core.Api.Setup;
using Notescrib.Core.Extensions;

namespace Notescrib.Identity.Api;

public class Module : IModule
{
    public void AddServices(WebApplicationBuilder builder)
    {
        builder.Services.AddControllers().ConfigureSerialization();

        if (builder.Environment.IsDevelopment())
        {
            builder.Services.ConfigureSwagger();
        }

        var jwtSettings = builder.Configuration.GetSettings<JwtSettings>()!;
        builder.Services.ConfigureJwtAuth(x =>
        {
            x.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
            x.ValidateIssuerSigningKey = true;
            x.ValidateIssuer = true;
            x.ValidIssuer = jwtSettings.Issuer;
            x.ValidateLifetime = true;
        });

        builder.Services.AddRequiredServices(builder.Configuration);
    }

    public void ConfigureApplication(WebApplication app)
    {
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseDeveloperExceptionPage();
        }
        else
        {
            app.UseMiddleware<ExceptionHandlingMiddleware>();
        }
        
        app.UseHttpsRedirection();
        
        app.UseAuthentication();
        app.UseAuthorization();
        
        app.MapControllers();
    }
}
