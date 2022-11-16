using FluentValidation;
using Notescrib.Notes.Contracts;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Features.Notes.Models;

public class NoteContentsSection : IChildrenCollectionTree<NoteContentsSection>
{
    public string Name { get; set; } = null!;
    public string? Content { get; set; }
    public ICollection<NoteContentsSection> Children { get; set; } = Array.Empty<NoteContentsSection>();
}

internal class NoteContentsSectionValidator : AbstractValidator<NoteContentsSection>
{
    public NoteContentsSectionValidator()
    {
        RuleFor(x => x.Name)
            .NotEmpty()
            .MaximumLength(Size.Name.Max);
    }
}
