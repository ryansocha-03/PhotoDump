using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;

namespace photodump_api.Features.UploadMedia;

public static class UploadMediaEndpoint
{
    public static void AddUploadMediaEndpoint(this WebApplication app)
    {
        app.MapPost("events/{eventId}/guest/{guestId}/media", Handler)
            .Produces<UploadMediaResponseDto>(StatusCodes.Status200OK);
    }

    private static async Task<Ok<UploadMediaResponseDto>> Handler(
        [FromRoute] int eventId, 
        [FromRoute] int guestId, 
        [FromBody] UploadMediaRequestDto requestData, 
        [FromServices] UploadMediaHandler handler)
    {
        var jobQueueResult = await handler.HandleAsync(new UploadMediaModel
        {
            Files = requestData.Files,
            guestId = guestId,
            eventId = eventId
        });
        
        return TypedResults.Ok(
            new UploadMediaResponseDto
            {   
                SignedUrl = jobQueueResult.ToString()
            }
        );
    }
}