namespace Identity.Services.Sessions;

public static class SessionConfiguration
{
    public static readonly string SessionHeaderName = "X-Session-Id";
    public static readonly string EventHeaderName = "X-Event-Public-Id";
    public static readonly int SessionDuration = 4;
}