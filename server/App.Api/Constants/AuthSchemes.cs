namespace App.Api.Constants;

/// <summary>
/// Defines constant names for implemented auth schemes.
/// </summary>
public static class AuthSchemes
{
    /// <summary>
    /// Auth scheme using database-backed sessions.
    /// </summary>
    public const string SessionAuth = "Session";
    
    /// <summary>
    /// Auth scheme using worker tokens.
    /// </summary>
    public const string WorkerAuth = "Worker";
}