using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Notescrib.Features.Folders;
using Notescrib.Features.Notes;
using Notescrib.Features.Templates;
using Notescrib.Features.Workspaces;

namespace Notescrib.Data;

public class NotescribDbContext : DbContext
{
    public NotescribDbContext(DbContextOptions<NotescribDbContext> options)
        : base(options)
    {
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new WorkspaceEntityConfig());
        modelBuilder.ApplyConfiguration(new FolderEntityConfig());
        
        modelBuilder.ApplyConfiguration(new NoteEntityConfig());
        modelBuilder.ApplyConfiguration(new NoteTagEntityConfig());
        modelBuilder.ApplyConfiguration(new NoteContentEntityConfig());
        
        modelBuilder.ApplyConfiguration(new NoteTemplateEntityConfig());
        modelBuilder.ApplyConfiguration(new NoteTemplateContentEntityConfig());
    }

    public DbSet<Workspace> Workspaces => Set<Workspace>();
    public DbSet<Folder> Folders => Set<Folder>();
    public DbSet<Note> Notes => Set<Note>();
    public DbSet<NoteTemplate> NoteTemplates => Set<NoteTemplate>();
}

internal class UserDbContextFactory : IDesignTimeDbContextFactory<NotescribDbContext>
{
    private const string DefaultConnection = @"User ID=postgres;Password=postgres;Host=localhost;Port=5432;Database=Notescrib;";

    public NotescribDbContext CreateDbContext(string[] args)
    {
        var options = new DbContextOptionsBuilder<NotescribDbContext>()
            .UseNpgsql(DefaultConnection)
            .Options;

        return new NotescribDbContext(options);
    }
}
