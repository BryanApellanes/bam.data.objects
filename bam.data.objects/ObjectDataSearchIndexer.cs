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
                    File.WriteAllLines(indexPath, keys);
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
            string indexPath = GetSearchIndexPath(type, property.PropertyName, valueHash);
            object fileLock = FileLocks.GetOrAdd(indexPath, _ => new object());

            lock (fileLock)
            {
                if (!File.Exists(indexPath))
                {
                    return;
                }

                HashSet<string> keys = new HashSet<string>(File.ReadAllLines(indexPath));
                if (keys.Remove(objectDataKey.Key!))
                {
                    if (keys.Count == 0)
                    {
                        File.Delete(indexPath);
                    }
                    else
                    {
                        File.WriteAllLines(indexPath, keys);
                    }
                }
            }
        });

        return Task.CompletedTask;
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
                    File.WriteAllLines(indexPath, keys);
                }
            }
        }
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
