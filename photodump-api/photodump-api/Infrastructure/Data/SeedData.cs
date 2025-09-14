using photodump_api.Domain;

namespace photodump_api.Infrastructure.Db.Data;

public static class SeedData
{
    public static void Initialize(AppDbContext context)
    {
        context.Database.EnsureCreated();

        if (context.Events.Any())
            return;

        var event1 = new Event{ Name = "Amanda and Joe's Wedding", EventDate = DateTime.UtcNow};

        var guest1 = new Guest { FirstName = "Amanda", LastName = "Socha", Event = event1};
        var guest2 = new Guest { FirstName = "Joe", LastName = "Katic", Event = event1 };
        var guest3 = new Guest { FirstName = "Jeffrey", LastName = "Socha", Event = event1 };
        var guest4 = new Guest { FirstName = "Christine", LastName = "Socha", Event = event1 };
        var guest5 = new Guest { FirstName = "Jacob", LastName = "Socha", Event = event1 };

        var media1 = new Media { Path = "cheese", Event = event1, Guest = guest1, IsPrivate = false};
        var media2 = new Media { Path = "cheese", Event = event1, Guest = guest1, IsPrivate = true};
        var media3 = new Media { Path = "cheese", Event = event1, Guest = guest2, IsPrivate = false};
        var media4 = new Media { Path = "cheese", Event = event1, Guest = guest2, IsPrivate = false};
        var media5 = new Media { Path = "cheese", Event = event1, Guest = guest3, IsPrivate = true};
        var media6 = new Media { Path = "cheese", Event = event1, Guest = guest4, IsPrivate = false};
        var media7 = new Media { Path = "cheese", Event = event1, Guest = guest4, IsPrivate = true};
        
        context.Events.AddRange(event1);
        context.Guests.AddRange(guest1, guest2, guest3, guest4, guest5);
        context.MediaData.AddRange(media1, media2, media3, media4, media5, media6, media7);
        
        context.SaveChanges();  
    }
}