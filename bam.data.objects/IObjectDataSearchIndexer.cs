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
    /// Re-indexes an object whose property values may have changed: removes index entries for
    /// properties of <paramref name="previous"/> whose values differ from (or are absent in)
    /// <paramref name="current"/>, then indexes <paramref name="current"/>.  Callers on the
    /// update path MUST use this instead of <see cref="IndexAsync"/> alone — indexing only the
    /// current state leaves stale entries mapping the object's prior values to its key, so a
    /// search for a superseded value (e.g. a rotated-away public key) would still surface the
    /// object.
    /// </summary>
    /// <param name="previous">The previously stored state of the object, or null when no prior state exists (falls back to a plain index).</param>
    /// <param name="current">The current state of the object to index.</param>
    /// <returns>The result of the index operation, including the number of properties indexed.</returns>
    Task<IObjectDataSearchIndexResult> ReindexAsync(IObjectData? previous, IObjectData current);

    /// <summary>
    /// Determines whether a search index exists for the specified type.  False for a legacy
    /// store whose objects were written before search indexing was in place (and for types with
    /// no indexed objects at all) — callers should treat a missing index as "unknown", not
    /// "no matches", and fall back to scanning rather than trusting an empty lookup result.
    /// </summary>
    /// <param name="type">The type to check for a search index.</param>
    /// <returns>True when a search index directory exists for the type.</returns>
    bool HasIndex(Type type);

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
