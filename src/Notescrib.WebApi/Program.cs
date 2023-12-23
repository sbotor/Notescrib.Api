using Notescrib;
using Notescrib.Core.Api.Configuration;
using Notescrib.Core.Api.Extensions;
using Notescrib.Core.Api.Middleware;
using Notescrib.Core.Extensions;
using Notescrib.WebApi.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().ConfigureSerialization();
builder.Services.AddHealthChecks();

if (builder.Environment.IsDevelopment())
{
    builder.Services.ConfigureSwagger();
}

var jwtSettings = builder.Configuration.GetSettings<JwtSettings>()!;
builder.Services.ConfigureJwtAuth(jwtSettings);
        
builder.Services.AddRequiredServices(builder.Configuration);

var corsConfigured = builder.Services.TryAddCors(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/health");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
        
if (corsConfigured)
{
    app.UseCors();
}

app.UseAuthentication();
app.UseAuthorization();
        
app.MapControllers();

app.MigrateDatabase();

app.Run();
