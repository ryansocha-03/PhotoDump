using Identity.Models;
using Identity.Services;
using Identity.Services.Sessions;
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
         .AddScheme<AuthenticationSchemeOptions, EventSessionAuthHandler>("SessionScheme", options => { });

      services.AddScoped<SessionService>();
   }
}