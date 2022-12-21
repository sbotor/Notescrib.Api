using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Notescrib.Core.Cqrs.Behaviors;
using Notescrib.Core.Extensions;
using Notescrib.Emails.Services;

namespace Notescrib.Emails;

public static class ServicesExtensions
{
    public static IServiceCollection AddRequiredServices(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddPipelineBehavior(typeof(LoggingBehavior<,>));
        services.AddMediatrWithValidation(typeof(ServicesExtensions).Assembly);

        services.ConfigureSettings<EmailSettings>(config);

        services.AddScoped<IEmailSender, EmailSender>();
        
        return services;
    }
}
