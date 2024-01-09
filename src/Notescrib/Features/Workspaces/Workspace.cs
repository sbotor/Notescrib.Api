using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Notescrib.Utils;

namespace Notescrib.Features.Workspaces;

public class Workspace
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public string OwnerId { get; set; } = null!;
    public DateTime Created { get; set; }
}

internal class WorkspaceEntityConfig : IEntityTypeConfiguration<Workspace>
{
    public void Configure(EntityTypeBuilder<Workspace> builder)
    {
        builder.HasKey(x => x.Id);
        
        builder.Property(x => x.OwnerId).HasMaxLength(Consts.OwnerId.MaxLength);
    }
}
