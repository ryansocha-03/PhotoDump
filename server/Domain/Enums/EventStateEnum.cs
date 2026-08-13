using System.Runtime.Serialization;

namespace Domain.Enums;

/// <summary>
/// Defines the enumeration values for an Event State.
/// </summary>
public enum EventStateEnum
{
    [EnumMember(Value = "Draft")]
    Draft,
    
    [EnumMember(Value = "Published")]
    Published,
    
    [EnumMember(Value = "Cancelled")]
    Cancelled,
    
    [EnumMember(Value = "Completed")]
    Completed
}