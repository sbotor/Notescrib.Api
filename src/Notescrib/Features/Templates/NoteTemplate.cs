using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notescrib.Features.Workspaces;
using Notescrib.Utils;

namespace Notescrib.Features.Templates;

public class NoteTemplate
{
    internal const string TableName = "NoteTemplates";
    
    public Guid Id { get; set; }
    public string Name { get; set; } = null!;
    public string OwnerId { get; set; } = null!;
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    
    public NoteTemplateContent Content { get; set; } = null!;
    
    public Guid WorkspaceId { get; set; }
}

public class NoteTemplateContent
{
    public string Content { get; set; } = string.Empty;
    public Guid Id { get; set; }
}

internal class NoteTemplateEntityConfig : IEntityTypeConfiguration<NoteTemplate>
{
    public void Configure(EntityTypeBuilder<NoteTemplate> builder)
    {
        builder.ToTable(NoteTemplate.TableName);
        builder.HasKey(x => x.Id);

        builder.HasOne(x => x.Content)
            .WithOne()
            .HasForeignKey<NoteTemplateContent>(x => x.Id);

        builder.Property(x => x.Name).HasMaxLength(Consts.Name.MaxLength);
        builder.Property(x => x.OwnerId).HasMaxLength(Consts.OwnerId.MaxLength);
        
        builder.HasOne<Workspace>()
            .WithMany()
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}

internal class NoteTemplateContentEntityConfig : IEntityTypeConfiguration<NoteTemplateContent>
{
    public void Configure(EntityTypeBuilder<NoteTemplateContent> builder)
    {
        builder.ToTable(NoteTemplate.TableName);
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.Content).HasMaxLength(Consts.NoteTemplate.MaxContentLength);
    }
}
