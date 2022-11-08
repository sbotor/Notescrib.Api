using Notescrib.Notes.Api.Extensions;

var app = WebApplication.CreateBuilder(args)
    .Configure()
    .Build();

app.Configure();

app.Run();
