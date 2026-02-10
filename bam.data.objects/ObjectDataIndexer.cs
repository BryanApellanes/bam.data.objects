namespace Bam.Data.Objects;

public class ObjectDataIndexer : IObjectDataIndexer
{
    public ObjectDataIndexer(IObjectDataStorageManager storageManager, ICompositeKeyCalculator compositeKeyCalculator)
    {
        this.StorageManager = storageManager;
        this.CompositeKeyCalculator = compositeKeyCalculator;
    }

    private IObjectDataStorageManager StorageManager { get; }
    private ICompositeKeyCalculator CompositeKeyCalculator { get; }

    public async Task<IObjectDataIndexResult> IndexAsync(object data)
    {
        if (data is IObjectData objectData)
        {
            return await IndexAsync(objectData);
        }

        return await IndexAsync(new ObjectData(data));
    }

    public async Task<IObjectDataIndexResult> IndexAsync(IObjectData data)
    {
        IObjectDataKey objectDataKey = data.GetObjectKey();
        ulong id = CompositeKeyCalculator.CalculateULongKey(data);

        string indexPath = GetIndexPath(data.TypeDescriptor.Type, id);
        FileInfo fileInfo = new FileInfo(indexPath);
        fileInfo.Directory?.Create();
        await File.WriteAllTextAsync(indexPath, objectDataKey.Key);

        return new ObjectDataIndexResult
        {
            Success = true,
            Id = id,
            ObjectDataKey = objectDataKey
        };
    }

    public Task<IObjectDataKey?> LookupAsync<T>(ulong id)
    {
        return LookupAsync(typeof(T), id);
    }

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
