using FluentValidation;
using Notescrib.Api.Core.Contracts;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Common.Validators;

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
