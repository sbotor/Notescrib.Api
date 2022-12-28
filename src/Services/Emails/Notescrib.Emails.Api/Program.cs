using Notescrib.Core.Api.Extensions;
using Notescrib.Core.Api.Middleware;
using Notescrib.Core.Extensions;
using Notescrib.Emails;
using Notescrib.Emails.Api;
using Notescrib.Emails.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers().ConfigureSerialization();
builder.Services.AddHealthChecks();

if (builder.Environment.IsDevelopment())
{
    builder.Services.ConfigureSwagger();
}

builder.Services.ConfigureSettings<AuthSettings>(builder.Configuration);
builder.Services.AddRequiredServices(builder.Configuration);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();
}

app.UseHealthChecks("/health");

app.UseMiddleware<ExceptionHandlingMiddleware>();

app.UseMiddleware<ApiKeyAuthMiddleware>();

app.MapControllers();

app.Run();
