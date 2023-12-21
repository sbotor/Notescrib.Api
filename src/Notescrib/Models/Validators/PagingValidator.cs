using FluentValidation;

namespace Notescrib.Models.Validators;

internal class PagingValidator : AbstractValidator<Paging>
{
    public PagingValidator()
    {
        RuleFor(x => x.Page)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(Paging.MaxPageSize);
    }
}
