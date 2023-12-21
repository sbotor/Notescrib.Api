using Notescrib.Core.Api.Configuration;
using Notescrib.Core.Api.Extensions;
using Notescrib.Core.Api.Middleware;
using Notescrib.Core.Extensions;
using Notescrib.Identity;
using Notescrib.Identity.Api;
using Notescrib.Identity.Api.Extensions;
using Notescrib.Identity.Api.Features.Auth;
using Notescrib.Identity.Api.Features.Auth.Providers;
using Notescrib.Identity.Features.Auth.Providers;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().ConfigureSerialization();
builder.Services.AddHealthChecks();

if (builder.Environment.IsDevelopment())
{
    builder.Services.ConfigureSwagger();
}

builder.Services.ConfigureSettings<IdentityJwtSettings>(builder.Configuration, nameof(JwtSettings));

var jwtSettings = builder.Configuration.GetSettings<IdentityJwtSettings>(nameof(JwtSettings))!;
builder.Services.ConfigureJwtAuth(jwtSettings);

builder.Services.AddRequiredServices(builder.Configuration);
builder.Services.AddTransient<IJwtProvider, JwtProvider>();

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

app.MigrateDatabase();
app.Run();
