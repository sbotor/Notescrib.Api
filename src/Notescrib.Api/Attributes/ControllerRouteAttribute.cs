using Microsoft.AspNetCore.Mvc;

namespace Notescrib.Api.Attributes;

public class ControllerRouteAttribute : RouteAttribute
{
    public ControllerRouteAttribute(string template = "[controller]")
        : base("api/" + template)
    {
    }
}
