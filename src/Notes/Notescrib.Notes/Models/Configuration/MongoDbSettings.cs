﻿namespace Notescrib.Notes.Application.Models.Configuration;

public class MongoDbSettings
{
    public string ConnectionUri { get; set; } = null!;
    public string DatabaseName { get; set; } = null!;
    public CollectionNames CollectionNames { get; set; } = new();
}

public class CollectionNames
{
    public string Workspaces { get; set; } = "Workspaces";
}