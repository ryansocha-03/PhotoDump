using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Text.Encodings.Web;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace Identity.Services.Workers;

public sealed class WorkerAuthHandler(
    IOptionsMonitor<WorkerAuthConfiguration> options,
    ILoggerFactory loggerFactory,
    ISystemClock clock,
    UrlEncoder encoder)
    : AuthenticationHandler<WorkerAuthConfiguration>(options, loggerFactory, encoder, clock)
{
    protected override Task<AuthenticateResult> HandleAuthenticateAsync()
    {
        if (!Request.Headers.TryGetValue(Options.HeaderName, out var headerValue))
            return Task.FromResult(AuthenticateResult.Fail("Missing worker token."));

        if (!CryptographicOperations.FixedTimeEquals(
                Encoding.UTF8.GetBytes(headerValue.ToString()),
                Encoding.UTF8.GetBytes(Options.Token)))
        {
            return Task.FromResult(AuthenticateResult.Fail("Invalid worker token."));
        }
        
        var identity = new ClaimsIdentity(
            [new Claim(ClaimTypes.Name, "worker")],
            Scheme.Name);
        
        var principal = new ClaimsPrincipal(identity);
        var ticket = new AuthenticationTicket(principal, Scheme.Name);
        
        return Task.FromResult(AuthenticateResult.Success(ticket));
    }
}