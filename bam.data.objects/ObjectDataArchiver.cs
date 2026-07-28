namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataArchiver"/>: moves an object's property
/// storage out of live storage into the <c>archive</c> area under the storage root and removes
/// the object's ID and UUID index entries, so the object stops resolving as live data (it no
/// longer appears in retrievals or enumerations) while its bytes remain on disk for manual
/// recovery.  The mirror image of <see cref="ObjectDataDeleter"/>, preserving instead of
/// destroying.  Search-index entries are NOT touched here — searchers verify loaded values, so
/// an archived object's dangling entries are inert; callers wanting a clean search index run
/// <see cref="IObjectDataSearchIndexer.RebuildAsync(System.Type)"/> after archiving.
/// </summary>
public class ObjectDataArchiver : IObjectDataArchiver
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectDataArchiver"/> class.
    /// </summary>
    /// <param name="factory">The factory used to create object data wrappers.</param>
    /// <param name="storageManager">The storage manager used to resolve storage paths.</param>
    /// <param name="compositeKeyCalculator">The composite key calculator used to compute IDs for index cleanup.</param>
    public ObjectDataArchiver(IObjectDataFactory factory, IObjectDataStorageManager storageManager, ICompositeKeyCalculator compositeKeyCalculator)
    {
        this.Factory = factory;
        this.StorageManager = storageManager;
        this.CompositeKeyCalculator = compositeKeyCalculator;
    }

    private IObjectDataFactory Factory { get; }
    private IObjectDataStorageManager StorageManager { get; }
    private ICompositeKeyCalculator CompositeKeyCalculator { get; }

    /// <inheritdoc />
    public Task<IObjectDataArchiveResult> ArchiveAsync(object data)
    {
        IObjectData objectData = Factory.GetObjectData(data);
        return ArchiveAsync(objectData);
    }

    /// <inheritdoc />
    public Task<IObjectDataArchiveResult> ArchiveAsync(IObjectData data)
    {
        try
        {
            IObjectDataKey objectDataKey = data.GetObjectKey();
            ulong id = CompositeKeyCalculator.CalculateULongKey(data);

            // Move the object's property storage directory into the archive area.
            ITypeStorageHolder typeHolder = StorageManager.GetObjectStorageHolder(data.TypeDescriptor.Type);
            List<string> keyParts = new List<string> { typeHolder.FullName! };
            keyParts.AddRange(objectDataKey.Key!.Split(2));
            string objectStoragePath = Path.Combine(keyParts.ToArray());

            string archivePath = GetArchivePath(data.TypeDescriptor, objectDataKey.Key!);
            if (Directory.Exists(objectStoragePath))
            {
                Directory.CreateDirectory(Path.GetDirectoryName(archivePath)!);
                Directory.Move(objectStoragePath, archivePath);
            }

            // Remove the id index entry so live enumerations no longer surface the object.
            string indexPath = GetIndexPath(data.TypeDescriptor, id);
            if (File.Exists(indexPath))
            {
                File.Delete(indexPath);
            }

            // Remove the uuid index entry if a uuid exists.
            string uuid = GetUuid(data.Data);
            if (!string.IsNullOrEmpty(uuid))
            {
                string uuidIndexPath = GetUuidIndexPath(data.TypeDescriptor, uuid);
                if (File.Exists(uuidIndexPath))
                {
                    File.Delete(uuidIndexPath);
                }
            }

            return Task.FromResult<IObjectDataArchiveResult>(new ObjectDataArchiveResult
            {
                Success = true,
                ArchivePath = archivePath
            });
        }
        catch (Exception ex)
        {
            return Task.FromResult<IObjectDataArchiveResult>(new ObjectDataArchiveResult
            {
                Success = false,
                Message = ProcessMode.Current.Mode == ProcessModes.Prod
                    ? ex.GetBaseException().Message
                    : ex.GetMessageAndStackTrace()
            });
        }
    }

    /// <summary>
    /// Builds the archive destination for an object's storage:
    /// <c>{root}/archive/objects/{Type.FullName parts}/{key}</c>, suffixed with an incrementing
    /// ordinal (<c>-1</c>, <c>-2</c>, …) when the same key has been archived before, so a
    /// re-created and re-archived object never collides with a prior archive.
    /// </summary>
    private string GetArchivePath(TypeDescriptor typeDescriptor, string key)
    {
        List<string> parts = new List<string>();
        parts.Add(StorageManager.GetRootStorageHolder().FullName!);
        parts.Add("archive");
        parts.Add("objects");
        string fullName = typeDescriptor.Type.FullName ?? "UNSPECIFIED_TYPE_NAME";
        parts.AddRange(fullName.Split('.'));
        parts.Add(key);
        string basePath = Path.Combine(parts.ToArray());

        string archivePath = basePath;
        int ordinal = 0;
        while (Directory.Exists(archivePath))
        {
            ordinal++;
            archivePath = $"{basePath}-{ordinal}";
        }

        return archivePath;
    }

    private static string GetUuid(object data)
    {
        System.Reflection.PropertyInfo uuidProp = data.GetType().GetProperty("Uuid")!;
        return (uuidProp?.GetValue(data) as string)!;
    }

    private string GetIndexPath(TypeDescriptor typeDescriptor, ulong id)
    {
        List<string> parts = new List<string>();
        parts.Add(StorageManager.GetRootStorageHolder().FullName!);
        parts.Add("index");
        string fullName = typeDescriptor.Type.FullName ?? "UNSPECIFIED_TYPE_NAME";
        parts.AddRange(fullName.Split('.'));
        parts.Add(id.ToString());
        return Path.Combine(parts.ToArray());
    }

    private string GetUuidIndexPath(TypeDescriptor typeDescriptor, string uuid)
    {
        List<string> parts = new List<string>();
        parts.Add(StorageManager.GetRootStorageHolder().FullName!);
        parts.Add("index-uuid");
        string fullName = typeDescriptor.Type.FullName ?? "UNSPECIFIED_TYPE_NAME";
        parts.AddRange(fullName.Split('.'));
        parts.Add(uuid);
        return Path.Combine(parts.ToArray());
    }
}
