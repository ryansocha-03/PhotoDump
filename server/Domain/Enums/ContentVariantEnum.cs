using System.Runtime.Serialization;

namespace Domain.Enums;

/// <summary>
/// Defines the enumeration values for content variants.
/// </summary>
public enum ContentVariantEnum
{
    [EnumMember(Value = "gallery")]
    Gallery,
    
    [EnumMember(Value = "spotlight")]
    Spotlight,
    
    [EnumMember(Value = "original")]
    Original
}