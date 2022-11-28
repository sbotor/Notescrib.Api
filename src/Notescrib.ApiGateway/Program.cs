using Notescrib.Core.Api.Configuration;
using Notescrib.Core.Api.Extensions;
using Notescrib.Core.Extensions;

var builder = WebApplication.CreateBuilder(args);

var yarpConfig = new ConfigurationBuilder()
    .AddJsonFile("yarp.json", true, true)
    .AddJsonFile($"yarp.{builder.Environment.EnvironmentName}.json", true, true)
    .Build();
builder.Services.AddReverseProxy().LoadFromConfig(yarpConfig);

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

app.MapReverseProxy();

app.Run();
