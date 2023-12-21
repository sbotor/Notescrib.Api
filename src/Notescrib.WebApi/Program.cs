using Notescrib;
using Notescrib.Core.Api.Configuration;
using Notescrib.Core.Api.Extensions;
using Notescrib.Core.Api.Middleware;
using Notescrib.Core.Extensions;

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

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHealthChecks("/health");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseHttpsRedirection();
        
app.UseAuthentication();
app.UseAuthorization();
        
app.MapControllers();

app.Run();
