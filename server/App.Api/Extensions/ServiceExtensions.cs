using App.Api.Constants;
using App.Api.Models.Configuration;
using App.Api.Services.Definition;
using App.Api.Services.Implementation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.AspNetCore.Identity;

namespace App.Api.Extensions;

/// <summary>
/// Provides extensions for configuring and registering App API services.
/// </summary>
public static class ServiceExtensions
{
    /// <summary>
    /// Registers required API-specific services. 
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> the add the services to.</param>
    public static void AddApiServices(this IServiceCollection services)
    {
        services.AddSingleton<IPasswordHasher<object>, PasswordHasher<object>>();
        
        services.AddScoped<IPasswordService, PasswordService>();
        services.AddScoped<IMediaService, MediaService>();
        services.AddScoped<IEventService, EventService>();
    }
    
    /// <summary>
    /// Registers services for performing data protection operations.
    /// </summary>
    /// <param name="builder">The <see cref="WebApplicationBuilder"/> to add the services to.</param>
    /// <param name="configurationSection">The path to the configuration section holding the keystore path. Defaults to 'DataProtection:KeystorePath'.</param>
    public static void AddDataProtectionServices(this WebApplicationBuilder builder, string configurationSection = "DataProtection:KeystorePath")
    {
        var dataProtectionBuilder = builder.Services.AddDataProtection();

        if (builder.Environment.IsProduction())
        {
            var dataProtectionKeyPath = builder.Configuration.GetValue<string?>(configurationSection);
            if (string.IsNullOrWhiteSpace(dataProtectionKeyPath)) throw new ArgumentNullException($"Keystore path is missing at {configurationSection}");

            dataProtectionBuilder.PersistKeysToFileSystem(new DirectoryInfo(dataProtectionKeyPath));
        }
    }
    
    /// <summary>
    /// Registers services, configurations, and auth schemes for session auth.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> containing the configuration values.</param>
    /// <param name="configurationSection">The path to the configuration section for session auth. Defaults to 'SessionAuth'.</param>
    public static void AddSessionAuth(this IServiceCollection services, IConfiguration configuration, string configurationSection = "SessionAuth")
    {
        services.AddScoped<ISessionService, SessionService>();
        
        services.AddOptions<SessionAuthConfiguration>()
            .Bind(configuration.GetSection("SessionAuth"));
        
        services.AddAuthentication(AuthSchemes.SessionAuth)
            .AddScheme<AuthenticationSchemeOptions, SessionAuthHandler>(AuthSchemes.SessionAuth, _ => { });
    }

    /// <summary>
    /// Registers services, configurations, and auth schemes for worker auth.
    /// </summary>
    /// <param name="services">The <see cref="IServiceCollection"/> to add the services to.</param>
    /// <param name="configuration">The <see cref="IConfiguration"/> containing the configuration values.</param>
    /// <param name="configurationSection">The path to the configuration section for worker auth. Defaults to 'WorkerAuth'.</param>
    public static void AddWorkerAuth(this IServiceCollection services, IConfiguration configuration,
        string configurationSection = "WorkerAuth")
    {
        services.AddOptions<WorkerAuthConfiguration>()
            .Bind(configuration.GetSection(configurationSection))
            .ValidateDataAnnotations()
            .ValidateOnStart();
        
        services.AddAuthentication(AuthSchemes.WorkerAuth)
            .AddScheme<WorkerAuthConfiguration, WorkerAuthHandler>(AuthSchemes.WorkerAuth, options =>
        {
            var workerToken = configuration[$"{configurationSection}:Token"]; 
            ArgumentNullException.ThrowIfNull(workerToken);
            
            options.Token = workerToken;
        });
    }
}