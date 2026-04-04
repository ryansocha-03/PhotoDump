using Microsoft.AspNetCore.Authentication;

namespace Identity.Services.Workers;

public sealed class WorkerAuthConfiguration : AuthenticationSchemeOptions
{
    public string Token { get; set; } = "";
    public string HeaderName { get; set; } = "X-Worker-Token";
}