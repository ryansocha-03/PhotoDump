using App.Api.Models;
using App.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace App.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class AuthController(AuthService authService) : ControllerBase
{
    [HttpPost("event-login")]
    public async Task<IActionResult> CheckEventPassword([FromBody] EventLoginRequestModel eventLoginRequest)
    {
        if (string.IsNullOrWhiteSpace(eventLoginRequest.EventKey))
            return BadRequest("EventKey is required");
        
        var signedInEvent = await authService.FindEventForPassword(eventLoginRequest.EventKey);
        if (signedInEvent is null)
            return Unauthorized("EPIC FART");
        
        var token = authService.CreateGuestTokenForEvent(signedInEvent);
        
        return Ok(token);
    }
}