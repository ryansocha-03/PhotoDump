using Domain.Enums;

namespace ContentStore.Abstractions.Models;

/// <summary>
/// Defines the fields for keying a group of objects in a Content Store.
/// </summary>
public sealed record ContentKeyGroup(Guid OwnerId, FilePrivacyEnum Privacy, ContentVariantEnum ContentVariant, IEnumerable<string> ObjectNames);

/// <summary>
/// Defines the fields for keying a single object in a Content Store.
/// </summary>
public sealed record ContentKey(Guid OwnerId, FilePrivacyEnum Privacy, ContentVariantEnum ContentVariant, string ObjectName);