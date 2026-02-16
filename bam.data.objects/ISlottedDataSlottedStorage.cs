using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Extends slotted storage with object-level save and load operations by hash.
/// </summary>
public interface ISlottedDataSlottedStorage : ISlottedStorage
{
    /// <summary>
    /// Saves an object to slotted storage.
    /// </summary>
    /// <param name="data">The object to save.</param>
    /// <returns>The result of the save operation.</returns>
    IObjectDataStorageSaveResult SaveObject(object data);

    /// <summary>
    /// Saves a strongly-typed object to slotted storage.
    /// </summary>
    /// <typeparam name="T">The type of the object to save.</typeparam>
    /// <param name="data">The object to save.</param>
    /// <returns>The result of the save operation.</returns>
    IObjectDataStorageSaveResult<T> SaveObject<T>(T data);

    /// <summary>
    /// Loads an object from slotted storage by its hash.
    /// </summary>
    /// <param name="hash">The hash identifying the object.</param>
    /// <returns>The result of the load operation.</returns>
    IObjectDataStorageLoadResult LoadObject(string hash);

    /// <summary>
    /// Loads a strongly-typed object from slotted storage by its hash.
    /// </summary>
    /// <typeparam name="T">The type of the object to load.</typeparam>
    /// <param name="hash">The hash identifying the object.</param>
    /// <returns>The result of the load operation.</returns>
    IObjectStorageLoadResult<T> LoadObject<T>(string hash);
}