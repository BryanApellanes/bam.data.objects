namespace Bam.Data.Objects;

/// <summary>
/// Defines operations for maintaining a property-value search index that maps hashed property values to object keys.
/// </summary>
public interface IObjectDataSearchIndexer
{
    /// <summary>
    /// Indexes all properties of the specified object data for search, mapping each property value hash to the object key.
    /// </summary>
    /// <param name="data">The object data to index.</param>
    /// <returns>The result of the index operation, including the number of properties indexed.</returns>
    Task<IObjectDataSearchIndexResult> IndexAsync(IObjectData data);

    /// <summary>
    /// Removes the specified object data from the search index.
    /// </summary>
    /// <param name="data">The object data to remove from the index.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RemoveAsync(IObjectData data);

    /// <summary>
    /// Looks up object keys that have the specified property value hash for the given type and property name.
    /// </summary>
    /// <param name="type">The type to search within.</param>
    /// <param name="propertyName">The property name to search by.</param>
    /// <param name="valueHash">The hashed value to search for.</param>
    /// <returns>An enumerable of matching object keys.</returns>
    Task<IEnumerable<IObjectDataKey>> LookupAsync(Type type, string propertyName, string valueHash);

    /// <summary>
    /// Rebuilds the entire search index for the specified type by re-reading all stored objects.
    /// </summary>
    /// <param name="type">The type to rebuild the search index for.</param>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RebuildAsync(Type type);

    /// <summary>
    /// Rebuilds the entire search index for the specified type by re-reading all stored objects.
    /// </summary>
    /// <typeparam name="T">The type to rebuild the search index for.</typeparam>
    /// <returns>A task representing the asynchronous operation.</returns>
    Task RebuildAsync<T>();
}
