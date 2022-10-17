using Notescrib.Api;
using Notescrib.Api.Infrastructure;
using Notescrib.Api.Middleware;

var app = WebApplication.CreateBuilder(args)
    .Configure()
    .Build();

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
