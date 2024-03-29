﻿using FluentValidation;

namespace Notescrib.Models;

public readonly struct Paging
{
    public const int DefaultPageSize = 10;
    public const int MaxPageSize = 100;

    public int Page { get; }
    public int PageSize { get; }

    public Paging(int? page = null, int? pageSize = null)
    {
        Page = page ?? 1;
        PageSize = pageSize ?? DefaultPageSize;
    }

    public class Validator : AbstractValidator<Paging>
    {
        public Validator()
        {
            RuleFor(x => x.Page)
                .GreaterThan(0);

            RuleFor(x => x.PageSize)
                .LessThanOrEqualTo(MaxPageSize);
        }
    }
}
