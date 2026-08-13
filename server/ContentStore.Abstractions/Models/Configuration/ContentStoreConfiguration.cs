using System.ComponentModel.DataAnnotations;

namespace ContentStore.Abstractions.Models.Configuration;

/// <summary>
/// Defines the configuration values necessary for adding a content store
/// </summary>
public sealed record ContentStoreConfiguration
{
    /// <summary>
    /// Defines the size of the window that uploads can be performed.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public required int UploadWindowMinutes { get; init; }
    
    /// <summary>
    /// Defines the size of the window that downloads can be performed.
    /// </summary>
    [Required]
    [Range(1, int.MaxValue)]
    public required int DownloadWindowMinutes { get; init; }
}