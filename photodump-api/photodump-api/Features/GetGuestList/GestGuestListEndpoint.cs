using Microsoft.AspNetCore.Http.HttpResults;
using photodump_api.Domain;

namespace photodump_api.Features.GetGuestList;

public static class GestGuestListEndpoint
{
    public static void AddGuestListEndpoint(this WebApplication app)
    {
        app.MapGet("events/{eventId}/guests", Handler)
            .Produces<List<GetGuestListDto>>(StatusCodes.Status200OK);
    }
    
    private static async Task<Ok<List<GetGuestListDto>>> Handler(int eventId, GetGuestListHandler handler)
    {
        List<Guest> guestsForEvent = await handler.HandleAsync(eventId);
        var finalGuestList = guestsForEvent.Select<Guest, GetGuestListDto>(guest =>
            new GetGuestListDto
            {
                Id = guest.Id,
                FirstName = guest.FirstName,
                LastName = guest.LastName,
            }
        ).ToList();
        return TypedResults.Ok(finalGuestList);
    }
}