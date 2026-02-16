using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Combines storage location, composite key, and content identifier to fully locate an object in storage.
/// </summary>
public interface IObjectDataLocator
{
    /// <summary>
    /// Gets the storage identifier indicating where the object is stored.
    /// </summary>
    IStorageIdentifier StorageIdentifier { get; }

    /// <summary>
    /// Gets the composite key for the object.
    /// </summary>
    IObjectDataKey ObjectDataKey { get; }

    /// <summary>
    /// Gets the content-based identifier for the object.
    /// </summary>
    IObjectDataIdentifier ObjectDataIdentifier { get; }
}