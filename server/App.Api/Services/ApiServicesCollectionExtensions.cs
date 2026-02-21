using Core.Interfaces;

namespace App.Api.Services;

public static class ApiServicesCollectionExtensions
{
   public static void AddApiServices(this IServiceCollection services)
   {
      services.AddScoped<EventService>();
      services.AddScoped<MediaService>();

      services.AddSingleton<IBrokerConnection, BrokerConnectionService>();
      services.AddScoped<IBrokerPublisher, BrokerService>();
   } 
}