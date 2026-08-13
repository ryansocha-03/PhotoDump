using System.Runtime.Serialization;
using Domain.Entities;

namespace Domain.Enums;

/// <summary>
/// Defines the enumeration values for content status.
/// </summary>
public enum ContentStatusEnum
{
    /// <summary>
    /// Indicates an upload has been requested for the <see cref="Media"/> but the file has not been physically uploaded.
    /// </summary>
    [EnumMember(Value = "Pending")]
    Pending,
    
    /// <summary>
    /// Indicates the <see cref="Media"/> has been physically uploaded but not yet processed.
    /// </summary>
    [EnumMember(Value = "Uploaded")]
    Uploaded,
    
    /// <summary>
    /// Indicates an uploaded <see cref="Media"/> is currently undergoing processing.
    /// </summary>
    [EnumMember(Value = "Processing")]
    Processing,
    
    /// <summary>
    /// Indicates a <see cref="Media"/> is uploaded, processed, and ready for users to consume.
    /// </summary>
    [EnumMember(Value = "Completed")]
    Completed,
    
    /// <summary>
    /// Indicates the media upload pipeline has failed for a particular <see cref="Media"/> 
    /// </summary>
    [EnumMember(Value = "Failed")]
    Failed,
    
    /// <summary>
    /// Indicates the upload has been cancelled for a particular <see cref="Media"/>
    /// </summary>
    [EnumMember(Value = "Cancelled")]
    Cancelled,
    
    /// <summary>
    /// Indicates a <see cref="Media"/> has been deleted.
    /// </summary>
    [EnumMember(Value = "Deleted")]
    Deleted
}