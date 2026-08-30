using Asp.Versioning;
using ContentStore.Abstractions.Interfaces;
using ContentStore.Abstractions.Models;
using Domain.Entities;
using Domain.Enums;
using Internal.Api.Models.Request;
using Microsoft.AspNetCore.Mvc;
using Persistence.Abstractions.Interfaces.Repositories;

namespace Internal.Api.Controllers;

/// <summary>
/// Provides endpoints for internal operations on media.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
public class MediaController(IContentStoreService contentStoreService, IMediaRepository mediaRepository) : ControllerBase
{
    /// <summary>
    /// Deletes a piece of media
    /// </summary>
    [HttpDelete]
    [ActionName("DeleteContentByExactName")]
    [ProducesResponseType(typeof(Media), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> DeleteMedia([FromBody] DeleteExactContentRequestModel requestData)
    {
        try
        {
            var deleteWasSuccessful = true;
            foreach (var contentToDelete in requestData.contentNames)
            {
                var currentDeleteResult = await contentStoreService.DeleteContentExactAsync(contentToDelete);
                if (!currentDeleteResult) deleteWasSuccessful = false;
            }
            return Ok(deleteWasSuccessful);
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Unexpected error when deleting exact content: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Gets all content.
    /// </summary>
    [HttpGet]
    [ActionName("GetAllContent")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetAllContent()
    {
        try
        {
            return Ok(await contentStoreService.GetContentNamesAsync(null));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Unexpected error when fetching content: {ex.Message}");
        }
    }

    [HttpGet("{eventPublicId}/{id:long}/download")]
    [ActionName("GetContentDownloadByExactName")]
    public async Task<IActionResult> GetContentDownloadByExactName([FromRoute] string eventPublicId,[FromRoute] long id)
    {
        var mediaToDownload = await mediaRepository.GetByIdAsync(id);
        if (mediaToDownload == null) return NotFound();

        return Ok(await contentStoreService.CreateDownloadsAsync(new ContentKeyGroup(Guid.Parse(eventPublicId),  FilePrivacyEnum.Private, ContentVariantEnum.Original, [mediaToDownload.PublicFileName])));
    }

    /// <summary>
    /// Gets all content at a specific path.
    /// </summary>
    [HttpGet("{path}")]
    [ActionName("GetContentByPath")]
    [ProducesResponseType(typeof(IEnumerable<string>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetContentByPath([FromRoute] string path)
    {
        try
        {
            return Ok(await contentStoreService.GetContentNamesAsync(path));
        }
        catch (Exception ex)
        {
            return StatusCode(StatusCodes.Status500InternalServerError, $"Unexpected error when fetching content: {ex.Message}");
        }
    }
}