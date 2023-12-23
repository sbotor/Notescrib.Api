using Microsoft.EntityFrameworkCore;
using Notescrib.Data;

namespace Notescrib.WebApi.Extensions;

public static class WebApplicationExtensions
{
    public static void MigrateDatabase(this WebApplication app)
    {
        var scope = app.Services.CreateScope();
        var dbContext = scope.ServiceProvider.GetRequiredService<NotescribDbContext>();
        dbContext.Database.Migrate();
    }
}
