using Microsoft.AspNetCore.Diagnostics.HealthChecks;
using Notescrib.Core.Api.Configuration;
using Notescrib.Core.Api.Extensions;
using Notescrib.Core.Extensions;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Configuration
    .AddJsonFile("ocelot.json")
    .AddJsonFile($"ocelot.{builder.Environment.EnvironmentName}.json", true, true);

builder.Services.AddOcelot();

var jwtSettings = builder.Configuration.GetSettings<JwtSettings>()!;
builder.Services.ConfigureJwtAuth(jwtSettings);

builder.Services.AddHealthChecks();

var allowedOrigins = builder.Configuration.GetSettings<string[]>("AllowedOrigins");
builder.Services.AddCors(options =>
    options.AddDefaultPolicy(policy => policy
        .WithOrigins(allowedOrigins!)
        .WithMethods("GET", "POST", "PUT", "DELETE")
        .AllowAnyHeader()));

var app = builder.Build();

app.UseCors();

app.UseHealthChecks("/health");

await app.UseOcelot();

app.Run();
