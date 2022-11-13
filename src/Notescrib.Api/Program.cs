using System.Text;
using Microsoft.IdentityModel.Tokens;
using Notescrib.Api.Models;
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
builder.Services.ConfigureJwtAuth(x =>
{
    x.IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.Key));
    x.ValidateIssuerSigningKey = true;
    x.ValidateIssuer = true;
    x.ValidIssuer = jwtSettings.Issuer;
    x.ValidateAudience = true;
    x.ValidAudience = jwtSettings.Audience;
    x.ValidateLifetime = true;
});

var app = builder.Build();

await app.UseOcelot();

app.Run();
