namespace Bam.Data.Objects;

/// <summary>
/// Factory for creating object data locators, keys, and identifiers from object data.
/// </summary>
public interface IObjectDataLocatorFactory
{
    /// <summary>
    /// Creates a complete locator for the specified object data, including storage location, key, and identifier.
    /// </summary>
    /// <param name="storageManager">The storage manager used to resolve storage locations.</param>
    /// <param name="data">The object data to locate.</param>
    /// <returns>A fully resolved object data locator.</returns>
    IObjectDataLocator GetObjectDataLocator(IObjectDataStorageManager storageManager, IObjectData data);

    /// <summary>
    /// Computes the composite key for the specified object data.
    /// </summary>
    /// <param name="data">The object data to compute the key for.</param>
    /// <returns>The computed object key.</returns>
    IObjectDataKey GetObjectKey(IObjectData data);

    /// <summary>
    /// Computes the content-based identifier for the specified object data.
    /// </summary>
    /// <param name="data">The object data to compute the identifier for.</param>
    /// <returns>The computed object identifier.</returns>
    IObjectDataIdentifier GetObjectIdentifier(IObjectData data);

}