using System.ComponentModel.DataAnnotations;

namespace Persistence.Abstractions.Models.Configuration;

/// <summary>
/// Defines the shape of the configuration for a database.
/// </summary>
public record DatabaseConfiguration
{
    [Required(AllowEmptyStrings = false)]
    public required string DatabaseProvider { get; init; }
    
    [Required(AllowEmptyStrings = false)]
    public required string ConnectionString { get; init; }
}