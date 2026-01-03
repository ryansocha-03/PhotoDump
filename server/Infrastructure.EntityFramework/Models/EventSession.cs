using System.Net;

namespace Infrastructure.EntityFramework.Models;

public class EventSession
{
    public Guid Id { get; set; }
    
    public Guid EventPublicId { get; set; }
    public int? GuestId { get; set; }
    
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset ExpiresAt { get; set; }
    public DateTimeOffset? RevokedAt { get; set; }
    public DateTimeOffset LastSeenAt { get; set; }
    
    public string? UserAgent { get; set; }
    public IPAddress? IpAddress { get; set; }
}