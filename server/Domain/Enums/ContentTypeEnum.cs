using System.Runtime.Serialization;

namespace Domain.Enums;

/// <summary>
/// Defines enumeration values for supported content types.
/// </summary>
public enum ContentTypeEnum
{
    [EnumMember(Value = "None")]
    None,
    
    [EnumMember(Value = "JPG")]
    Jpg,
    
    [EnumMember(Value = "PNG")] 
    Png,
    
    [EnumMember(Value = "WEBP")]
    Webp,
    
    [EnumMember(Value = "AVIF")]
    Avif,
    
    [EnumMember(Value = "JPEG")]
    Jpeg,
    
    [EnumMember(Value = "HEIC")]
    Heic,
    
    [EnumMember(Value = "HEIF")]
    Heif
}