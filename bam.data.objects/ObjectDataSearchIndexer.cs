using System.Collections.Concurrent;

namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataSearchIndexer"/> that maintains a file-system-based search index mapping hashed property values to object keys.
/// </summary>
public class ObjectDataSearchIndexer : IObjectDataSearchIndexer
{
    private static readonly ConcurrentDictionary<string, object> FileLocks = new();

    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectDataSearchIndexer"/> class.
    /// </summary>
    /// <param name="storageManager">The storage manager used to resolve storage paths and read objects.</param>
    /// <param name="indexer">The indexer used to enumerate all keys when rebuilding the search index.</param>
    public ObjectDataSearchIndexer(IObjectDataStorageManager storageManager, IObjectDataIndexer indexer)
    {
        this.StorageManager = storageManager;
        this.Indexer = indexer;
    }

    private IObjectDataStorageManager StorageManager { get; }
    private IObjectDataIndexer Indexer { get; }

    /// <inheritdoc />
    public Task<IObjectDataSearchIndexResult> IndexAsync(IObjectData data)
    {
        IObjectDataKey objectDataKey = data.GetObjectKey();
        Type type = data.TypeDescriptor.Type;
        IProperty[] properties = data.Properties.ToArray();
        int indexed = 0;

        Parallel.ForEach(properties, property =>
        {
            string valueHash = ComputeValueHash(property.Value);
            string indexPath = GetSearchIndexPath(type, property.PropertyName, valueHash);
            object fileLock = FileLocks.GetOrAdd(indexPath, _ => new object());

            lock (fileLock)
            {
                FileInfo fi = new FileInfo(indexPath);
                fi.Directory?.Create();

                HashSet<string> keys = File.Exists(indexPath)
                    ? new HashSet<string>(File.ReadAllLines(indexPath))
                    : new HashSet<string>();

                if (keys.Add(objectDataKey.Key!))
                {
                    WriteAllLinesAtomic(indexPath, keys);
                }
            }

            Interlocked.Increment(ref indexed);
        });

        return Task.FromResult<IObjectDataSearchIndexResult>(new ObjectDataSearchIndexResult
        {
            Success = true,
            PropertiesIndexed = indexed
        });
    }

    /// <inheritdoc />
    public Task RemoveAsync(IObjectData data)
    {
        IObjectDataKey objectDataKey = data.GetObjectKey();
        Type type = data.TypeDescriptor.Type;
        IProperty[] properties = data.Properties.ToArray();

        Parallel.ForEach(properties, property =>
        {
            string valueHash = ComputeValueHash(property.Value);
            RemoveEntry(type, property.PropertyName, valueHash, objectDataKey.Key!);
        });

        return Task.CompletedTask;
    }

    /// <inheritdoc />
    public async Task<IObjectDataSearchIndexResult> ReindexAsync(IObjectData? previous, IObjectData current)
    {
        ArgumentNullException.ThrowIfNull(current);

        // Add before remove: a crash between the two leaves a stale old-value entry that
        // verify-on-read filters, instead of a window where neither value resolves.
        IObjectDataSearchIndexResult result = await IndexAsync(current);
        if (previous != null)
        {
            RemoveStaleEntries(previous, current);
        }

        return result;
    }

    /// <inheritdoc />
    public bool HasIndex(Type type)
    {
        return Directory.Exists(GetSearchIndexDirectoryForType(type));
    }

    /// <inheritdoc />
    public bool HasIndex(Type type, string propertyName)
    {
        return Directory.Exists(Path.Combine(GetSearchIndexDirectoryForType(type), propertyName));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IObjectDataKey>> LookupAsync(Type type, string propertyName, string valueHash)
    {
        string indexPath = GetSearchIndexPath(type, propertyName, valueHash);
        if (!File.Exists(indexPath))
        {
            return Enumerable.Empty<IObjectDataKey>();
        }

        string[] hexKeys = await File.ReadAllLinesAsync(indexPath);
        return hexKeys
            .Where(k => !string.IsNullOrEmpty(k))
            .Select(key => new ObjectDataKey
            {
                TypeDescriptor = new TypeDescriptor(type),
                Key = key
            });
    }

    /// <inheritdoc />
    public async Task RebuildAsync<T>()
    {
        await RebuildAsync(typeof(T));
    }

    /// <inheritdoc />
    public async Task RebuildAsync(Type type)
    {
        string searchIndexDir = GetSearchIndexDirectoryForType(type);
        if (Directory.Exists(searchIndexDir))
        {
            Directory.Delete(searchIndexDir, true);
        }

        IEnumerable<IObjectDataKey> allKeys = await Indexer.GetAllKeysAsync(type);
        IObjectDataKey[] keyArray = allKeys.ToArray();

        Parallel.ForEach(keyArray, key =>
        {
            IObjectData objectData = StorageManager.ReadObject(key);
            IndexSync(objectData);
        });
    }

    private void IndexSync(IObjectData data)
    {
        IObjectDataKey objectDataKey = data.GetObjectKey();
        Type type = data.TypeDescriptor.Type;

        foreach (IProperty property in data.Properties)
        {
            string valueHash = ComputeValueHash(property.Value);
            string indexPath = GetSearchIndexPath(type, property.PropertyName, valueHash);
            object fileLock = FileLocks.GetOrAdd(indexPath, _ => new object());

            lock (fileLock)
            {
                FileInfo fi = new FileInfo(indexPath);
                fi.Directory?.Create();

                HashSet<string> keys = File.Exists(indexPath)
                    ? new HashSet<string>(File.ReadAllLines(indexPath))
                    : new HashSet<string>();

                if (keys.Add(objectDataKey.Key!))
                {
                    WriteAllLinesAtomic(indexPath, keys);
                }
            }
        }
    }

    /// <summary>
    /// Removes index entries for properties of <paramref name="previous"/> whose value hashes
    /// differ from (or have no counterpart in) <paramref name="current"/>.  A property whose
    /// value became null disappears from <see cref="IObjectData.Properties"/> (null values are
    /// never enumerated), so its prior entry is removed via the missing-counterpart path.
    /// Iterates sequentially (unlike <see cref="IndexAsync"/>/<see cref="RemoveAsync"/>) —
    /// typically only a few properties change per update, below parallelization overhead.
    /// </summary>
    private void RemoveStaleEntries(IObjectData previous, IObjectData current)
    {
        IObjectDataKey previousKey = previous.GetObjectKey();
        Type type = previous.TypeDescriptor.Type;
        Dictionary<string, string> currentValueHashes = current.Properties
            .ToDictionary(property => property.PropertyName, property => ComputeValueHash(property.Value));

        foreach (IProperty property in previous.Properties)
        {
            string previousValueHash = ComputeValueHash(property.Value);
            if (currentValueHashes.TryGetValue(property.PropertyName, out string? currentValueHash)
                && string.Equals(currentValueHash, previousValueHash, StringComparison.Ordinal))
            {
                continue;
            }

            RemoveEntry(type, property.PropertyName, previousValueHash, previousKey.Key!);
        }
    }

    private void RemoveEntry(Type type, string propertyName, string valueHash, string objectKey)
    {
        string indexPath = GetSearchIndexPath(type, propertyName, valueHash);
        object fileLock = FileLocks.GetOrAdd(indexPath, _ => new object());

        lock (fileLock)
        {
            if (!File.Exists(indexPath))
            {
                return;
            }

            HashSet<string> keys = new HashSet<string>(File.ReadAllLines(indexPath));
            if (keys.Remove(objectKey))
            {
                if (keys.Count == 0)
                {
                    File.Delete(indexPath);
                }
                else
                {
                    WriteAllLinesAtomic(indexPath, keys);
                }
            }
        }
    }

    /// <summary>
    /// Writes an index file atomically: the content lands in a temp file first and is moved
    /// over the target in one operation, so a crash mid-write can never leave a truncated
    /// index file that silently drops other objects' keys.  Callers hold the per-file lock.
    /// </summary>
    private static void WriteAllLinesAtomic(string indexPath, IEnumerable<string> lines)
    {
        string tempPath = $"{indexPath}.tmp-{Guid.NewGuid():N}";
        File.WriteAllLines(tempPath, lines);
        File.Move(tempPath, indexPath, overwrite: true);
    }

    private string GetSearchIndexDirectoryForType(Type type)
    {
        List<string> parts = new List<string>();
        parts.Add(StorageManager.GetRootStorageHolder().FullName!);
        parts.Add("search-index");
        string fullName = type.FullName ?? "UNSPECIFIED_TYPE_NAME";
        parts.AddRange(fullName.Split('.'));
        return Path.Combine(parts.ToArray());
    }

    private string GetSearchIndexPath(Type type, string propertyName, string valueHash)
    {
        List<string> parts = new List<string>();
        parts.Add(StorageManager.GetRootStorageHolder().FullName!);
        parts.Add("search-index");
        string fullName = type.FullName ?? "UNSPECIFIED_TYPE_NAME";
        parts.AddRange(fullName.Split('.'));
        parts.Add(propertyName);
        parts.Add(valueHash);
        return Path.Combine(parts.ToArray());
    }

    internal static string ComputeValueHash(string encodedValue)
    {
        return (encodedValue ?? "null").HashHexString(HashAlgorithms.SHA256);
    }
}
