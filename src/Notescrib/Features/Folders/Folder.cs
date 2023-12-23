using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notescrib.Features.Notes;
using Notescrib.Features.Workspaces;
using Notescrib.Utils;

namespace Notescrib.Features.Folders;

public class Folder
{
    public Guid Id { get; set; } = Guid.NewGuid();
    
    public string OwnerId { get; set; } = null!;
    public string Name { get; set; } = null!;
    public DateTime Created { get; set; }
    public DateTime? Updated { get; set; }
    public int NestingLevel { get; set; }

    public ICollection<Note> Notes { get; set; } = null!;
    
    public Guid? ParentId { get; set; }
    public ICollection<Folder> Children { get; set; } = null!;
    
    public Guid WorkspaceId { get; set; }
}

internal class FolderEntityConfig : IEntityTypeConfiguration<Folder>
{
    public void Configure(EntityTypeBuilder<Folder> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.OwnerId).HasMaxLength(Consts.OwnerId.MaxLength);
        builder.Property(x => x.Name).HasMaxLength(Consts.Name.MaxLength);

        builder.HasOne<Folder>()
            .WithMany(x => x.Children)
            .HasForeignKey(x => x.ParentId)
            .OnDelete(DeleteBehavior.Cascade);

        builder.HasOne<Workspace>()
            .WithMany()
            .HasForeignKey(x => x.WorkspaceId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
