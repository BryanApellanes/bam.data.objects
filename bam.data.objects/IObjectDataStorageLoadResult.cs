using Bam.Data.Objects;

namespace Bam.Storage;

/// <summary>
/// Represents the result of loading object data from storage.
/// </summary>
public interface IObjectDataStorageLoadResult
{
    /// <summary>
    /// Gets the loaded object data, or null if the load failed.
    /// </summary>
    IObjectData Data { get; }

    /// <summary>
    /// Gets a value indicating whether the load operation succeeded.
    /// </summary>
    bool Success { get; }

    /// <summary>
    /// Gets a message describing the result, typically populated on failure.
    /// </summary>
    string Message { get; }
}