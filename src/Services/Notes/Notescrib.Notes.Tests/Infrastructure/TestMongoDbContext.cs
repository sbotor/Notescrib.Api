using Moq;
using Notescrib.Notes.Data.MongoDb;
using Notescrib.Notes.Features.Folders.Repositories;
using Notescrib.Notes.Features.Notes.Repositories;
using Notescrib.Notes.Features.Templates.Repositories;
using Notescrib.Notes.Features.Workspaces.Repositories;

namespace Notescrib.Notes.Tests.Infrastructure;

public class TestMongoDbContext : IMongoDbContext
{
    public IWorkspaceRepository Workspaces => Mocks.Workspaces.Object;
    public IFolderRepository Folders => Mocks.Folders.Object;
    public INoteRepository Notes => Mocks.Notes.Object;
    public INoteTemplateRepository NoteTemplates => Mocks.NoteTemplates.Object;

    public MongoDbRepositoryMocks Mocks { get; } = new();
    
    public ValueTask EnsureTransactionAsync(CancellationToken cancellationToken = default)
        => ValueTask.CompletedTask;

    public ValueTask CommitTransactionAsync()
        => ValueTask.CompletedTask;

    public void Dispose()
    {
    }

    public ValueTask DisposeAsync()
        => ValueTask.CompletedTask;
}

public class MongoDbRepositoryMocks
{
    public Mock<IWorkspaceRepository> Workspaces { get; } = new();
    public Mock<IFolderRepository> Folders { get; } = new();
    public Mock<INoteRepository> Notes { get; } = new();
    public Mock<INoteTemplateRepository> NoteTemplates { get; } = new();
}
