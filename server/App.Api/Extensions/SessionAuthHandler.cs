using System.Security.Claims;
using System.Text.Encodings.Web;
using App.Api.Constants;
using App.Api.Services.Definition;
using App.Api.Services.Implementation;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;

namespace App.Api.Extensions;

public sealed class SessionAuthHandler(IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock,
    ISessionService sessionService)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(SessionAuthHeaders.SessionHeader, out var sessionId) 
            || !Guid.TryParse(sessionId.ToString(), out var sessionGuid)
            || !Request.Headers.TryGetValue(SessionAuthHeaders.EventHeader, out var eventId)
            || !Guid.TryParse(eventId.ToString(), out var eventGuid)
           ) 
            return AuthenticateResult.Fail("Session missing or invalid"); 
        
        if (await sessionService.GetValidSessionAsync(sessionGuid, eventGuid) == null)
            return AuthenticateResult.Fail("Invalid session");

        var claims = new[]
        {
            new Claim("SessionId", sessionId.ToString()),
            new Claim("EventId", eventId.ToString())
        };
        var identity = new ClaimsIdentity(claims, Scheme.Name);
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        
        return AuthenticateResult.Success(ticket);
    }

    protected override async Task HandleChallengeAsync(AuthenticationProperties properties)
    {
        Response.StatusCode = 401;
        Response.ContentType = "text/plain";
        await Response.WriteAsync("Unauthorized");
    }
}