using Notescrib.Core.Api.Extensions;
using Notescrib.Identity.Api;
using Notescrib.Identity.Api.Extensions;

var app = WebApplication.CreateBuilder(args)
    .UseModule<Module>();

app.MigrateDatabase();

app.Run();
