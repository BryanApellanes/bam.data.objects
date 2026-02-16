namespace Bam.Data.Objects;

/// <summary>
/// Represents the result of indexing an object, including the assigned composite key ID and object key.
/// </summary>
public interface IObjectDataIndexResult
{
    /// <summary>
    /// Gets a value indicating whether the index operation succeeded.
    /// </summary>
    bool Success { get; }

    /// <summary>
    /// Gets the ulong composite key ID assigned to the indexed object.
    /// </summary>
    ulong Id { get; }

    /// <summary>
    /// Gets the object data key of the indexed object.
    /// </summary>
    IObjectDataKey ObjectDataKey { get; }
}
