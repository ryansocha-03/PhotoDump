using Core.Interfaces;

namespace App.Api.Services;

public static class ApiServicesCollectionExtensions
{
   public static void AddApiServices(this IServiceCollection services)
   {
      services.AddDataProtection();
      services.AddSingleton<MediaCursorService>();
      
      services.AddScoped<EventService>();
      services.AddScoped<MediaService>();

      services.AddSingleton<IBrokerConnection, BrokerConnectionService>();
      services.AddScoped<IBrokerPublisher, BrokerService>();
   } 
}