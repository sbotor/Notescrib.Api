using System.Text;
using Microsoft.IdentityModel.Tokens;
using Notescrib.Core.Api.Extensions;
using Notescrib.Core.Api.Middleware;
using Notescrib.Core.Extensions;
using Notescrib.Notes;
using Notescrib.Notes.Api.Models;

var builder = WebApplication.CreateBuilder(args);

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
    x.ValidateAudience = true;
    x.ValidAudience = jwtSettings.Audience;
    x.ValidateLifetime = true;
});
        
builder.Services.AddRequiredServices(builder.Configuration);

var app = builder.Build();

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

app.Run();
