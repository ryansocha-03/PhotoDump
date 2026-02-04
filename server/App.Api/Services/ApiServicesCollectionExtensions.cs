namespace App.Api.Services;

public static class ApiServicesCollectionExtensions
{
   public static void AddApiServices(this IServiceCollection services)
   {
      services.AddScoped<EventService>();
      services.AddScoped<MediaService>();
   } 
}