using FluentValidation;
using Notescrib.Notes.Models.Enums;
using Notescrib.Notes.Utils;

namespace Notescrib.Notes.Models;

public class SharingInfo
{
    public VisibilityLevel Visibility { get; set; } = VisibilityLevel.Private;
    public ICollection<string> AllowedIds { get; set; } = new List<string>();
}

internal class SharingInfoValidator : AbstractValidator<SharingInfo>
{
    public SharingInfoValidator()
    {
        RuleFor(x => x.AllowedIds.Count)
            .LessThanOrEqualTo(Consts.Note.MaxSharingCount);
        RuleForEach(x => x.AllowedIds)
            .NotEmpty();
    }
}
