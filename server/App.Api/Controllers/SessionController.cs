using App.Api.Constants;
using App.Api.Models.Request;
using App.Api.Models.Response;
using App.Api.Services.Definition;
using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

/// <summary>
/// Provides endpoints for creating and validating existing sessions.
/// </summary>
[ApiController]
[ApiVersion("1.0")]
[Route("v{version:apiVersion}/[controller]")]
public class SessionController(
    IEventService eventService,
    IPasswordService passwordService,
    ISessionService sessionService,
    ILogger<SessionController> logger)
    : ControllerBase
{
    /// <summary>
    /// Logs the user into the provided event using the provided event password, creating a
    /// new user session if the password is valid.
    /// </summary>
    /// <param name="eventPublicId">
    /// The public ID of the event to check the password, and create the session (if applicable) for.
    /// </param>
    /// <param name="eventLoginRequest">
    /// The <see cref="EventLoginRequestModel"/> containing the user-entered event password.
    /// </param>
    [HttpPost]
    [ActionName("EventLogin")]
    [ProducesResponseType(typeof(SessionResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> EventLogin(
        [FromHeader(Name = SessionAuthHeaders.EventHeader)] Guid eventPublicId,
        [FromBody] EventLoginRequestModel eventLoginRequest)
    {
        if (eventPublicId == Guid.Empty)
            return BadRequest("Missing required fields.");
        try
        {
            var eventHash = await eventService.GetEventPasswordHashAsync(eventPublicId);
            if (eventHash is null) return NotFound($"No event with public ID {eventPublicId}");

            if (!passwordService.PasswordMatchesHash(eventLoginRequest.EventKey, eventHash))
            {
                return Unauthorized("Invalid event key.");
            }

            var newSession = await sessionService.CreateSessionAsync(eventPublicId);
            return Ok(new SessionResponseModel
            {
                SessionId = newSession.Item1.ToString(),
                ExpiresAt = newSession.Item2
            });
        }
        catch (Exception ex)
        {
            logger.LogError($"Exception when verifying event password for event with ID: {eventPublicId}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occured.");
        }
    }    
    
    /// <summary>
    /// Gets the details for the provided session.
    /// </summary>
    [HttpGet]
    [ActionName("GetSessionDetails")]
    [ProducesResponseType(typeof(SessionResponseModel), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(string), StatusCodes.Status401Unauthorized)]
    [ProducesResponseType(typeof(string), StatusCodes.Status500InternalServerError)]
    public async Task<IActionResult> GetSessionDetails(
        [FromHeader(Name = SessionAuthHeaders.SessionHeader)] Guid sessionIdHeader,
        [FromHeader(Name = SessionAuthHeaders.EventHeader)] Guid eventPublicIdHeader)
    {
        if (sessionIdHeader == Guid.Empty ||  sessionIdHeader == Guid.Empty)
            return Unauthorized("Invalid session.");

        try
        {
            var sessionDetails = await sessionService.GetValidSessionAsync(sessionIdHeader, eventPublicIdHeader);
            if (sessionDetails is null)
                return Unauthorized("Invalid session.");

            return Ok(new SessionResponseModel
            {
                SessionId = sessionDetails.Value.Item1.ToString(),
                ExpiresAt = sessionDetails.Value.Item2
            });
        }
        catch (Exception ex)
        {
            logger.LogError($"Exception occured when validating session. Session ID: {sessionIdHeader}, Event Public ID: {eventPublicIdHeader}", ex.Message);
            return StatusCode(StatusCodes.Status500InternalServerError, "An unexpected error occured.");
        }
    }
}