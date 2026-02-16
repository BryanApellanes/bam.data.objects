namespace Bam.Data.Objects;

/// <summary>
/// Defines operations for indexing object data by composite key ID and UUID, and for looking up object keys from those indices.
/// </summary>
public interface IObjectDataIndexer
{
    /// <summary>
    /// Indexes the specified data object by computing its composite key and writing the index entry asynchronously.
    /// </summary>
    /// <param name="data">The object to index.</param>
    /// <returns>The result of the index operation.</returns>
    Task<IObjectDataIndexResult> IndexAsync(object data);

    /// <summary>
    /// Indexes the specified object data by computing its composite key and writing the index entry asynchronously.
    /// </summary>
    /// <param name="data">The object data to index.</param>
    /// <returns>The result of the index operation.</returns>
    Task<IObjectDataIndexResult> IndexAsync(IObjectData data);

    /// <summary>
    /// Looks up an object key by its composite key ulong ID for the specified type.
    /// </summary>
    /// <typeparam name="T">The type of object to look up.</typeparam>
    /// <param name="id">The composite key ID to search for.</param>
    /// <returns>The object key, or null if not found.</returns>
    Task<IObjectDataKey?> LookupAsync<T>(ulong id);

    /// <summary>
    /// Looks up an object key by its composite key ulong ID for the specified type.
    /// </summary>
    /// <param name="type">The type of object to look up.</param>
    /// <param name="id">The composite key ID to search for.</param>
    /// <returns>The object key, or null if not found.</returns>
    Task<IObjectDataKey?> LookupAsync(Type type, ulong id);

    /// <summary>
    /// Looks up an object key by its UUID for the specified type.
    /// </summary>
    /// <typeparam name="T">The type of object to look up.</typeparam>
    /// <param name="uuid">The UUID to search for.</param>
    /// <returns>The object key, or null if not found.</returns>
    Task<IObjectDataKey?> LookupByUuidAsync<T>(string uuid);

    /// <summary>
    /// Looks up an object key by its UUID for the specified type.
    /// </summary>
    /// <param name="type">The type of object to look up.</param>
    /// <param name="uuid">The UUID to search for.</param>
    /// <returns>The object key, or null if not found.</returns>
    Task<IObjectDataKey?> LookupByUuidAsync(Type type, string uuid);

    /// <summary>
    /// Gets all indexed object keys for the specified type.
    /// </summary>
    /// <param name="type">The type to retrieve keys for.</param>
    /// <returns>An enumerable of all indexed object keys for the type.</returns>
    Task<IEnumerable<IObjectDataKey>> GetAllKeysAsync(Type type);

    /// <summary>
    /// Gets all indexed object keys for the specified type.
    /// </summary>
    /// <typeparam name="T">The type to retrieve keys for.</typeparam>
    /// <returns>An enumerable of all indexed object keys for the type.</returns>
    Task<IEnumerable<IObjectDataKey>> GetAllKeysAsync<T>();
}
