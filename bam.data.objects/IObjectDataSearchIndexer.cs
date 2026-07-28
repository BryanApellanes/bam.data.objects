namespace Bam.Data.Objects;

/// <summary>
/// Defines operations for maintaining a property-value search index that maps hashed property values to object keys.
/// </summary>
/// <remarks>
/// Consistency contract implementations honor and callers must not assume more than:
/// <list type="bullet">
/// <item><description><b>In-process locking only.</b> Index mutations are serialized per index file within one
/// process; multiple processes writing one store can interleave read-modify-write cycles and lose entries.
/// Multi-process writers are unsupported.</description></item>
/// <item><description><b>Non-transactional with data writes.</b> Indexing is a separate operation from persisting
/// object data; a crash between the two can leave a stored object with no index entry (invisible to indexed
/// lookups until <see cref="RebuildAsync(System.Type)"/> runs) or an index entry with no stored object
/// (harmless — readers verify loaded values).</description></item>
/// <item><description><b><see cref="RebuildAsync(System.Type)"/> is not crash-safe.</b> It deletes the type's
/// index directory and re-indexes incrementally; a crash mid-rebuild leaves a partial index that reports
/// present via <see cref="HasIndex(System.Type)"/>.</description></item>
/// <item><description><b>An empty lookup result is not proof of absence.</b> Rows written before search indexing
/// existed, written by a crashed process, or predating a newly added property have no entries. Stores migrated
/// onto search indexing MUST run <see cref="RebuildAsync(System.Type)"/> once per type; security-sensitive
/// consumers must not treat an empty indexed result as authoritative without compensating controls.</description></item>
/// </list>
/// </remarks>
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
    /// Re-indexes an object whose property values may have changed: indexes
    /// <paramref name="current"/> FIRST, then removes index entries for properties of
    /// <paramref name="previous"/> whose values differ from (or are absent in) the current
    /// state.  Add-before-remove ordering means a crash mid-reindex leaves at worst a stale
    /// old-value entry (filtered by verify-on-read) rather than a window where neither value
    /// resolves.  Callers on the update path MUST use this instead of <see cref="IndexAsync"/>
    /// alone — indexing only the current state leaves stale entries mapping the object's prior
    /// values to its key, so a search for a superseded value (e.g. a rotated-away public key)
    /// would still surface the object.
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
    /// Determines whether a search index exists for the specified property of the type.  False
    /// when no object with a non-null value for the property has ever been indexed — callers
    /// should fall back to scanning rather than trusting an empty lookup for a never-indexed
    /// property (e.g. one newly added to the type).
    /// </summary>
    /// <param name="type">The type to check.</param>
    /// <param name="propertyName">The property to check for index entries.</param>
    /// <returns>True when a search index directory exists for the property.</returns>
    bool HasIndex(Type type, string propertyName);

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
