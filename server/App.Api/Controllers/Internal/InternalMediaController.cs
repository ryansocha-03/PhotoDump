using App.Api.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Internal;

[ApiController]
[Route("internal/media")]
[Authorize(AuthenticationSchemes = "WorkerScheme")]
public class InternalMediaController(MediaService mediaService): ControllerBase
{
    [HttpPost("{mediaId:int}/complete")]
    public async Task<IActionResult> CompleteMediaProcessing([FromRoute] int mediaId)
    {
        var updatedMedia = await mediaService.AcknowledgeCompleteStateTransition(mediaId);

        // TODO: Add better handling for 0 or 2+ rows updated
        return updatedMedia.Count switch
        {
            0 => NoContent(),
            1 => Ok(),
            _ => Ok("Multiple shis updated?")
        };
    }
}