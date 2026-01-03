using System.Security.Claims;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace Identity.Services.Sessions;

public class EventSessionAuthHandler(
    IOptionsMonitor<AuthenticationSchemeOptions> options,
    ILoggerFactory logger,
    UrlEncoder encoder,
    ISystemClock clock,
    SessionService sessionService)
    : AuthenticationHandler<AuthenticationSchemeOptions>(options, logger, encoder, clock)
{
    protected override async Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(SessionConfiguration.SessionHeaderName, out var sessionId) 
            || !Guid.TryParse(sessionId.ToString(), out var sessionGuid)
            || !Request.Headers.TryGetValue(SessionConfiguration.EventHeaderName, out var eventId)
            || !Guid.TryParse(eventId.ToString(), out var eventGuid)
        ) 
            return AuthenticateResult.Fail("Session missing or invalid"); 
        
        if (!await sessionService.ValidateSessionAsync(sessionGuid, eventGuid))
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