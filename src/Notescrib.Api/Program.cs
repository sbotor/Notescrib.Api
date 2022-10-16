using System.Text.Json.Serialization;
using Notescrib.Api.Application;
using Notescrib.Api.Extensions;
using Notescrib.Api.Infrastructure;
using Notescrib.Api.Middleware;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        var converters = options.JsonSerializerOptions.Converters;

        converters.Add(new JsonStringEnumConverter());
    });

if (builder.Environment.IsDevelopment())
{
    builder.Services.ConfigureSwagger();
}

builder.Services
    .AddApplicationServices(builder.Configuration)
    .AddInfrastructure(builder.Configuration);

builder.Services.ConfigureAuthentication(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();

    app.UseDeveloperExceptionPage();
}
else
{
    app.UseMiddleware<ErrorHandlingMiddleware>();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.MigrateDatabase();

app.Run();
