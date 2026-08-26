using System.Runtime.Serialization;

namespace Domain.Enums;

/// <summary>
/// Defines the enumeration values for file privacy.
/// </summary>
public enum FilePrivacyEnum
{
    [EnumMember(Value = "unknown")]
    Unknown,
    
    [EnumMember(Value = "public")]
    Public,
    
    [EnumMember(Value = "private")]
    Private,
}