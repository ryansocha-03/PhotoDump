using System.ComponentModel.DataAnnotations;

namespace ContentStore.MinIO.Models.Configuration;

/// <summary>
/// Defines the configuration values necessary for setting up a MinIO content store.
/// </summary>
public record MinIoConfiguration
{
    [Required(AllowEmptyStrings = false)]
    public required string AccessKey { get; init; }
    
    [Required(AllowEmptyStrings = false)]
    public required string SecretKey { get; init; }
    
    [Required(AllowEmptyStrings = false)]
    public required string InternalEndpoint { get; init; }
    
    [Required(AllowEmptyStrings = false)]
    public required string ExternalEndpoint { get; init; }
    
    [Required(AllowEmptyStrings = false)]
    public required string Region { get; init; }
    
    [Required(AllowEmptyStrings = false)]
    public required string Bucket { get; init; }

    public bool UseSsl { get; init; } = true;
}