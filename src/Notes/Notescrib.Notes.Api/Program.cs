using Notescrib.Core.Api.Extensions;
using Notescrib.Notes.Api;

var app = WebApplication.CreateBuilder(args)
    .UseModule<Module>();

app.Run();
