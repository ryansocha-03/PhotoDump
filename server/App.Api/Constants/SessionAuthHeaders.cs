namespace App.Api.Constants;

/// <summary>
/// Defines constant names for session auth headers.
/// </summary>
public static class SessionAuthHeaders
{
    /// <summary>
    /// The name of the header storing the session ID.
    /// </summary>
    public const string SessionHeader = "X-Session-Id";

    /// <summary>
    /// The name of the header storing the event ID.
    /// </summary>
    public const string EventHeader = "X-Event-Id";
}