namespace Bam.Storage;

/// <summary>
/// Defines file-system-based object storage operations for saving and loading objects by hash.
/// </summary>
public interface IFsObjectStorage
{
    /// <summary>
    /// Saves an object to the file system under the specified root path.
    /// </summary>
    /// <param name="rootPath">The root directory path for storage.</param>
    /// <param name="data">The object to save.</param>
    /// <returns>The result of the save operation.</returns>
    IObjectDataStorageSaveResult SaveObject(string rootPath, object data);

    /// <summary>
    /// Saves a strongly-typed object to the file system under the specified root path.
    /// </summary>
    /// <typeparam name="T">The type of the object to save.</typeparam>
    /// <param name="rootPath">The root directory path for storage.</param>
    /// <param name="data">The object to save.</param>
    /// <returns>The result of the save operation.</returns>
    IObjectDataStorageSaveResult<T> SaveObject<T>(string rootPath, T data);

    /// <summary>
    /// Loads an object from the file system using the specified hash.
    /// </summary>
    /// <param name="rootPath">The root directory path for storage.</param>
    /// <param name="hash">The hash identifying the object to load.</param>
    /// <returns>The result of the load operation.</returns>
    IObjectDataStorageLoadResult LoadObject(string rootPath, string hash);

    /// <summary>
    /// Loads a strongly-typed object from the file system using the specified hash.
    /// </summary>
    /// <typeparam name="T">The type of the object to load.</typeparam>
    /// <param name="rootPath">The root directory path for storage.</param>
    /// <param name="hash">The hash identifying the object to load.</param>
    /// <returns>The result of the load operation.</returns>
    IObjectStorageLoadResult<T> LoadObject<T>(string rootPath, string hash);
}