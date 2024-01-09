using System.Text.Json;
using Microsoft.AspNetCore.Identity;
using Notescrib.Core.Models;

namespace Notescrib.Identity.Extensions;

public static class IdentityResultExtensions
{
    public static IEnumerable<ErrorModel> ToErrorModels(this IdentityResult result)
        => result.Errors
            .Select(x => new ErrorModel(x.Code, x.Description))
            .ToArray();
}
