using Microsoft.EntityFrameworkCore;
using Notescrib.Identity.Data;

namespace Notescrib.Identity.Api.Extensions;

public static class WebApplicationExtensions
{
    public static WebApplication MigrateDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        using var context = scope.ServiceProvider.GetRequiredService<UserDbContext>();

        context.Database.Migrate();

        return app;
    }
}
