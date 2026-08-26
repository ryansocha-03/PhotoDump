using App.Api.Constants;
using App.Api.Models.DTOs;
using App.Api.Services.Definition;
using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers.Internal;

/// <summary>
/// Provides internal endpoints for interacting with media.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/internal/media")]
[Authorize(AuthenticationSchemes = AuthSchemes.WorkerAuth)]
public class InternalMediaController(IMediaService mediaService, ILogger<InternalMediaController> logger): ControllerBase
{
    /// <summary>
    /// Acknowledges the completion of the media processing pipeline for a particular media and makes it available for users. 
    /// </summary>
    [HttpPost("{mediaId:long}/complete")]
    [ActionName("CompleteMediaProcessing")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> CompleteMediaProcessing([FromRoute] long mediaId)
    {
        MediaStateTransitionDto? updatedMedia;
        try
        {
            updatedMedia = await mediaService.AcknowledgeMediaProcessingCompletionAsync(mediaId);
        }
        catch (InvalidOperationException ex)
        {
            logger.LogError(ex.Message, "Invalid number of media updated.");
            return BadRequest("Invalid number of media updated");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Unexpected error occurred.");
            return StatusCode(StatusCodes.Status500InternalServerError, "Unexpected error occurred");
        }

        if (updatedMedia == null)
        {
            return BadRequest("Invalid media id");
        }

        return Ok();
    }
}