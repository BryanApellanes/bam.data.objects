using System.Reflection;

namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataIndexer"/> that maintains file-system-based indices mapping composite key IDs and UUIDs to object keys.
/// </summary>
public class ObjectDataIndexer : IObjectDataIndexer
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectDataIndexer"/> class.
    /// </summary>
    /// <param name="storageManager">The storage manager used to resolve the root storage path for index files.</param>
    /// <param name="compositeKeyCalculator">The composite key calculator used to compute ulong IDs for indexing.</param>
    public ObjectDataIndexer(IObjectDataStorageManager storageManager, ICompositeKeyCalculator compositeKeyCalculator)
    {
        this.StorageManager = storageManager;
        this.CompositeKeyCalculator = compositeKeyCalculator;
    }

    private IObjectDataStorageManager StorageManager { get; }
    private ICompositeKeyCalculator CompositeKeyCalculator { get; }

    /// <inheritdoc />
    public async Task<IObjectDataIndexResult> IndexAsync(object data)
    {
        if (data is IObjectData objectData)
        {
            return await IndexAsync(objectData);
        }

        return await IndexAsync(new ObjectData(data));
    }

    /// <inheritdoc />
    public async Task<IObjectDataIndexResult> IndexAsync(IObjectData data)
    {
        IObjectDataKey objectDataKey = data.GetObjectKey();
        ulong id = CompositeKeyCalculator.CalculateULongKey(data);

        string indexPath = GetIndexPath(data.TypeDescriptor.Type, id);
        FileInfo fileInfo = new FileInfo(indexPath);
        fileInfo.Directory?.Create();
        await File.WriteAllTextAsync(indexPath, objectDataKey.Key);

        string uuid = GetUuid(data.Data);
        if (!string.IsNullOrEmpty(uuid))
        {
            string uuidIndexPath = GetUuidIndexPath(data.TypeDescriptor.Type, uuid);
            FileInfo uuidFileInfo = new FileInfo(uuidIndexPath);
            uuidFileInfo.Directory?.Create();
            await File.WriteAllTextAsync(uuidIndexPath, objectDataKey.Key);
        }

        return new ObjectDataIndexResult
        {
            Success = true,
            Id = id,
            ObjectDataKey = objectDataKey
        };
    }

    /// <inheritdoc />
    public Task<IObjectDataKey?> LookupAsync<T>(ulong id)
    {
        return LookupAsync(typeof(T), id);
    }

    /// <inheritdoc />
    public async Task<IObjectDataKey?> LookupAsync(Type type, ulong id)
    {
        string indexPath = GetIndexPath(type, id);
        if (!File.Exists(indexPath))
        {
            return null;
        }

        string hexKey = await File.ReadAllTextAsync(indexPath);
        return new ObjectDataKey
        {
            TypeDescriptor = new TypeDescriptor(type),
            Key = hexKey
        };
    }

    /// <inheritdoc />
    public Task<IObjectDataKey?> LookupByUuidAsync<T>(string uuid)
    {
        return LookupByUuidAsync(typeof(T), uuid);
    }

    /// <inheritdoc />
    public async Task<IObjectDataKey?> LookupByUuidAsync(Type type, string uuid)
    {
        string indexPath = GetUuidIndexPath(type, uuid);
        if (!File.Exists(indexPath))
        {
            return null;
        }

        string hexKey = await File.ReadAllTextAsync(indexPath);
        return new ObjectDataKey
        {
            TypeDescriptor = new TypeDescriptor(type),
            Key = hexKey
        };
    }

    /// <inheritdoc />
    public Task<IEnumerable<IObjectDataKey>> GetAllKeysAsync<T>()
    {
        return GetAllKeysAsync(typeof(T));
    }

    /// <inheritdoc />
    public async Task<IEnumerable<IObjectDataKey>> GetAllKeysAsync(Type type)
    {
        string indexDirectory = GetIndexDirectoryPath(type);
        List<IObjectDataKey> keys = new List<IObjectDataKey>();
        if (!Directory.Exists(indexDirectory))
        {
            return keys;
        }

        foreach (string filePath in Directory.GetFiles(indexDirectory))
        {
            string hexKey = await File.ReadAllTextAsync(filePath);
            keys.Add(new ObjectDataKey
            {
                TypeDescriptor = new TypeDescriptor(type),
                Key = hexKey
            });
        }

        return keys;
    }

    private string GetIndexDirectoryPath(Type type)
    {
        List<string> parts = new List<string>();
        parts.Add(StorageManager.GetRootStorageHolder().FullName);
        parts.Add("index");
        string fullName = type.FullName ?? "UNSPECIFIED_TYPE_NAME";
        parts.AddRange(fullName.Split('.'));
        return Path.Combine(parts.ToArray());
    }

    private static string GetUuid(object data)
    {
        PropertyInfo uuidProp = data.GetType().GetProperty("Uuid");
        return uuidProp?.GetValue(data) as string;
    }

    private string GetUuidIndexPath(Type type, string uuid)
    {
        List<string> parts = new List<string>();
        parts.Add(StorageManager.GetRootStorageHolder().FullName);
        parts.Add("index-uuid");
        string fullName = type.FullName ?? "UNSPECIFIED_TYPE_NAME";
        parts.AddRange(fullName.Split('.'));
        parts.Add(uuid);
        return Path.Combine(parts.ToArray());
    }

    private string GetIndexPath(Type type, ulong id)
    {
        List<string> parts = new List<string>();
        parts.Add(StorageManager.GetRootStorageHolder().FullName);
        parts.Add("index");
        string fullName = type.FullName ?? "UNSPECIFIED_TYPE_NAME";
        parts.AddRange(fullName.Split('.'));
        parts.Add(id.ToString());
        return Path.Combine(parts.ToArray());
    }
}
