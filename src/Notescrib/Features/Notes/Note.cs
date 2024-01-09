using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notescrib.Features.Folders;
using Notescrib.Features.Workspaces;
using Notescrib.Models;
using Notescrib.Models.Enums;
using Notescrib.Utils;

namespace Notescrib.Features.Notes;

public class Note
{
    internal const string TableName = "Notes";
    
    public Guid Id { get; set; } = Guid.NewGuid();
    public string Name { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public VisibilityLevel Visibility { get; set; }
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }

    public Guid WorkspaceId { get; set; }

    public Folder Folder { get; set; } = null!;
    public Guid FolderId { get; set; }

    public NoteContent Content { get; set; } = null!;
    public ICollection<NoteTag> Tags { get; set; } = null!;
    
    public ICollection<RelatedNote> RelatedNotes { get; set; } = null!;

    public SharingInfo GetSharingInfo()
        => new() { Visibility = Visibility };
}

public class NoteContent
{
    public string Content { get; set; } = string.Empty;
    public Guid Id { get; set; }
}

public class NoteTag
{
    public string Value { get; set; } = null!;
    public Guid NoteId { get; set; }
}

public class RelatedNote
{
    public Guid NoteId { get; set; }
    public Guid RelatedId { get; set; }
}

internal class NoteEntityConfig : IEntityTypeConfiguration<Note>
{
    public void Configure(EntityTypeBuilder<Note> builder)
    {
        builder.ToTable(Note.TableName);
        builder.HasKey(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(Consts.Name.MaxLength);
        builder.Property(x => x.OwnerId).HasMaxLength(Consts.OwnerId.MaxLength);

        builder.HasOne(x => x.Content)
            .WithOne()
            .HasForeignKey<NoteContent>(x => x.Id);
        
        builder.HasMany(x => x.Tags)
            .WithOne()
            .HasForeignKey(x => x.NoteId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Workspace>()
            .WithMany()
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
        
        builder.HasOne(x => x.Folder)
            .WithMany(x => x.Notes)
            .HasForeignKey(x => x.FolderId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasMany<Note>()
            .WithMany()
            .UsingEntity<RelatedNote>(
                l => l.HasOne<Note>()
                    .WithMany(x => x.RelatedNotes)
                    .HasForeignKey(x => x.NoteId),
                r => r.HasOne<Note>()
                    .WithMany()
                    .HasForeignKey(x => x.RelatedId));
    }
}

internal class NoteContentEntityConfig : IEntityTypeConfiguration<NoteContent>
{
    public void Configure(EntityTypeBuilder<NoteContent> builder)
    {
        builder.ToTable(Note.TableName);
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Content).HasMaxLength(Consts.Note.MaxContentLength);
    }
}

internal class NoteTagEntityConfig : IEntityTypeConfiguration<NoteTag>
{
    public void Configure(EntityTypeBuilder<NoteTag> builder)
    {
        builder.HasKey(x => new { x.NoteId, x.Value });

        builder.Property(x => x.Value).HasMaxLength(Consts.Note.MaxTagLength);
    }
}
