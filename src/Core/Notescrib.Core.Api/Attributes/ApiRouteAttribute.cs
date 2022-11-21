using Microsoft.AspNetCore.Mvc;

namespace Notescrib.Core.Api.Attributes;

public class ApiRouteAttribute : RouteAttribute
{
    public ApiRouteAttribute(string template = "[controller]")
        : base($"api/{template}")
    {
    }
}
