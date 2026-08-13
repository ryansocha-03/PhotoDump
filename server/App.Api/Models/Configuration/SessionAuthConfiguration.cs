namespace App.Api.Models.Configuration;

/// <summary>
/// Defines the configuration values necessary for configuring session auth.
/// </summary>
public record SessionAuthConfiguration
{
    public int SessionDurationMinutes { get; set; } = 240;
}