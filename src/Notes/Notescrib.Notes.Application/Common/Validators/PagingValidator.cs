using FluentValidation;
using Notescrib.Notes.Core.Contracts;
using Notescrib.Notes.Core.Models;

namespace Notescrib.Notes.Application.Common.Validators;

internal class PagingValidator : AbstractValidator<IPaging>
{
    public PagingValidator()
    {
        RuleFor(x => x.PageNumber)
            .GreaterThan(0);

        RuleFor(x => x.PageSize)
            .GreaterThan(0)
            .LessThanOrEqualTo(Paging.MaxPageSize);
    }
}
