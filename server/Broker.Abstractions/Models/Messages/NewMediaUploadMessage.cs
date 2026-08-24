using System.Text.Json.Serialization;

namespace Broker.Abstractions.Models.Messages;

/// <summary>
/// Defines the message shape for a new media upload.
/// </summary>
public record NewMediaUploadMessage
{
    [JsonPropertyName("ObjectName")]
    public required string ObjectName { get; set; }
    
    [JsonPropertyName("ObjectId")]
    public required long MediaId { get; set; }
}