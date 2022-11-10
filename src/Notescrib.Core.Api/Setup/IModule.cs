using Microsoft.AspNetCore.Builder;

namespace Notescrib.Core.Api.Setup;

public interface IModule
{
    void AddServices(WebApplicationBuilder builder);
    void ConfigureApplication(WebApplication app);
}
