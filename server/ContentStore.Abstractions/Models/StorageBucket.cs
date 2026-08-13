namespace ContentStore.Abstractions.Models;

/// <summary>
/// A bucket for storing content. 
/// </summary>
public sealed record StorageBucket(string Name, DateTime CreationDate);