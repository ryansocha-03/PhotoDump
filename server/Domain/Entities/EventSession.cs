using System.Net;

namespace Domain.Entities;

/// <summary>
/// Entity model defining an Event Session.
/// </summary>
public class EventSession
{
    public Guid Id { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public DateTimeOffset? LastSeenAt { get; set; }
    public string? UserAgent { get; set; }
    public IPAddress? IpAddress { get; set; } 
    
    #region Foreign Keys
    
    public Guid EventPublicId { get; set; }
    
    #endregion
    
    public Event? Event { get; private set; }
}