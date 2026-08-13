using System.Runtime.Serialization;

namespace Domain.Extensions;

/// <summary>
/// Defines extension methods for Enums.
/// </summary>
public static class EnumExtensions
{
    /// <summary>
    /// Gets the value of the EnumMember attribute for a given enum value.
    /// </summary>
    /// <param name="value">The enum value.</param>
    /// <typeparam name="TEnum">The type of the enum.</typeparam>
    /// <returns>The value of the EnumMember attribute if present; otherwise, the enum value as a string.</returns>
    public static string ToMemberValue<TEnum>(this TEnum value) where TEnum : Enum
    {
        var enumType = typeof(TEnum);
        var memberInfo = enumType.GetMember(value.ToString()).FirstOrDefault();
        
        var attribute = memberInfo?.GetCustomAttributes(typeof(EnumMemberAttribute), false)
            .FirstOrDefault() as EnumMemberAttribute;
        
        return attribute?.Value ?? value.ToString();
    }
}