using FluentValidation;
using FluentValidation.Results;
using Notescrib.Api.Core.Models;

namespace Notescrib.Api.Application.Extensions;

public static class ValidationExtensions
{
    public static IReadOnlyCollection<ErrorItem> Validate<T>(this IEnumerable<IValidator<T>> validators, T item)
    {
        var context = new ValidationContext<T>(item);
        return validators.Select(validator => validator.Validate(context))
            .GetErrors();
    }

    public static async Task<IReadOnlyCollection<ErrorItem>> ValidateAsync<T>(this IEnumerable<IValidator<T>> validators, T item)
    {
        var context = new ValidationContext<T>(item);
        var results = await Task.WhenAll(validators.Select(validator => validator.ValidateAsync(context)));

        return results.GetErrors();

    }

    private static IReadOnlyCollection<ErrorItem> GetErrors(this IEnumerable<ValidationResult> results)
        => results.Where(x => !x.IsValid)
            .SelectMany(x => x.Errors)
            .GroupBy(
                x => x.PropertyName,
                x => x.ErrorMessage,
                (propName, messages) => new ErrorItem
                {
                    Key = propName,
                    Messages = messages.Distinct().ToList()
                })
            .ToList();
}
