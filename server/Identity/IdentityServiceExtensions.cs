using Identity.Models;
using Identity.Services;
using Identity.Services.Sessions;
using Identity.Services.Workers;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Identity;

public static class IdentityServiceExtensions
{
   public static void AddIdentityServices(this IServiceCollection services)
   {
      services.AddScoped<PasswordService>();
   }

   public static void AddSessionAuth(this IServiceCollection services, IConfiguration configuration)
   {
      services.Configure<SessionAuthConfigurationModel>(configuration.GetSection("SessionAuth"));
      
      services.AddAuthentication("SessionScheme")
         .AddScheme<AuthenticationSchemeOptions, EventSessionAuthHandler>("SessionScheme", _ => { });

      services.AddScoped<SessionService>();
   }

   public static void AddWorkerAuth(this IServiceCollection services, IConfiguration configuration)
   {
      services.Configure<WorkerAuthConfiguration>(configuration.GetSection("WorkerAuth"));

      services.AddAuthentication().AddScheme<WorkerAuthConfiguration, WorkerAuthHandler>("WorkerAuth", options =>
      {
         options.HeaderName = configuration["WorkerAuth:HeaderName"] ?? "X-Worker-Auth";
         options.Token = configuration["WorkerAuth:Token"]!;
      });
   }
}