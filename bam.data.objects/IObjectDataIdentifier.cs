using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Represents a content-based identifier for an object, derived from a full hash of the object's data.
/// </summary>
public interface IObjectDataIdentifier
{
    /// <summary>
    /// Gets the type descriptor for the identified object.
    /// </summary>
    TypeDescriptor TypeDescriptor { get; }

    /// <summary>
    /// Resolves the storage identifier for this object using the specified storage manager.
    /// </summary>
    /// <param name="objectDataStorageManager">The storage manager to use for resolution.</param>
    /// <returns>The storage identifier for this object.</returns>
    IStorageIdentifier GetStorageIdentifier(IObjectDataStorageManager objectDataStorageManager);

    /// <summary>
    /// Gets the content hash identifier for the object.
    /// </summary>
    string? Id { get; }
}