using Domain.Enums;

namespace App.Api.Models.DTOs;

/// <summary>
/// DTO containing fields to identify a Media entity's state transition. 
/// </summary>
public record MediaStateTransitionDto
{
    public required long MediaInternalId { get; init; }
    public required FilePrivacyEnum Privacy { get; init; }
}