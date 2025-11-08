using Identity.Services;
using Microsoft.Extensions.DependencyInjection;

namespace Identity;

public static class IdentityServiceExtensions
{
   public static void AddIdentityServices(this IServiceCollection services)
   {
      services.AddScoped<PasswordService>();
      services.AddScoped<TokenService>();
   }
}