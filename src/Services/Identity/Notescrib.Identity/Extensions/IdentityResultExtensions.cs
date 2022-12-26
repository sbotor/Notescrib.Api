using System.Text.Json;
using Microsoft.AspNetCore.Identity;

namespace Notescrib.Identity.Extensions;

public static class IdentityResultExtensions
{
    public static string SerializeErrors(this IdentityResult result)
    {
        var errors = result.Errors
            .ToDictionary(x => x.Code, x => x.Description);
        return JsonSerializer.Serialize(errors);
    }
}
