using System.Text.Json.Serialization;
using Notescrib.Notes.Api.Middleware;

namespace Notescrib.Notes.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplicationBuilder Configure(this WebApplicationBuilder builder)
    {
        var services = builder.Services;

        services.AddControllers().AddJsonOptions(options =>
        {
            var conv = options.JsonSerializerOptions.Converters;
            conv.Add(new JsonStringEnumConverter());
        });
        
        if (builder.Environment.IsDevelopment())
        {
            builder.Services.ConfigureSwagger();
        }

        services.ConfigureAuthentication(builder.Configuration);

        services.AddRequiredServices(builder.Configuration);
        
        return builder;
    }

    public static IApplicationBuilder Configure(this WebApplication app)
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
        
        return app;
    }
}
