using Microsoft.AspNetCore.Identity;

namespace Internal.Api.Extensions;

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
    } 
}