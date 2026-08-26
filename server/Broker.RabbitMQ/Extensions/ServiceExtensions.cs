using Broker.RabbitMQ.Models.Configuration;
using MassTransit;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Broker.RabbitMQ.Extensions;

/// <summary>
/// Provides service extensions for configuring and registering a RabbitMQ broker.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Adds configuration options and registers services for a RabbitMQ message broker.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add services and configuration options to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> containing the configuration values.</param>
    /// <param name="configurationSection">The path the RabbitMQ configuration. Defaults to 'RabbitMQ'</param>
    public static void AddRabbitMqBroker(this IServiceCollection services, IConfiguration configuration, string configurationSection = "RabbitMQ")
    {
        services.AddOptions<RabbitMqClientConfiguration>()
            .Bind(configuration.GetSection(configurationSection))
            .ValidateDataAnnotations()
            .ValidateOnStart();

        services.AddMassTransit(x =>
        {
            x.UsingRabbitMq((context, config) =>
            {
                var rabbitOptions = context.GetRequiredService<IOptions<RabbitMqClientConfiguration>>().Value;
                
                config.Host(rabbitOptions.Host, "/", h =>
                {
                    h.Username(rabbitOptions.UserName);
                    h.Password(rabbitOptions.Password);
                });
                
                config.UseRawJsonSerializer();
                
                config.ConfigureJsonSerializerOptions(options => options);
                
                config.ConfigureEndpoints(context);
            });
        });
    }
}