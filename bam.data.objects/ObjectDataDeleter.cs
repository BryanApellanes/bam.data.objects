using System.Reflection;

namespace Bam.Data.Objects;

public class ObjectDataDeleter : IObjectDataDeleter
{
    public ObjectDataDeleter(IObjectDataFactory factory, IObjectDataStorageManager storageManager, ICompositeKeyCalculator compositeKeyCalculator)
    {
        this.Factory = factory;
        this.StorageManager = storageManager;
        this.CompositeKeyCalculator = compositeKeyCalculator;
    }

    private IObjectDataFactory Factory { get; }
    private IObjectDataStorageManager StorageManager { get; }
    private ICompositeKeyCalculator CompositeKeyCalculator { get; }

    public Task<IObjectDataDeleteResult> DeleteAsync(object data)
    {
        IObjectData objectData = Factory.GetObjectData(data);
        return DeleteAsync(objectData);
    }

    public async Task<IObjectDataDeleteResult> DeleteAsync(IObjectData data)
    {
        try
        {
            IObjectDataKey objectDataKey = data.GetObjectKey();
            ulong id = CompositeKeyCalculator.CalculateULongKey(data);

            // Delete the object's property storage directory
            ITypeStorageHolder typeHolder = StorageManager.GetObjectStorageHolder(objectDataKey.TypeDescriptor);
            List<string> keyParts = new List<string> { typeHolder.FullName };
            keyParts.AddRange(objectDataKey.Key.Split(2));
            string objectStoragePath = Path.Combine(keyParts.ToArray());
            if (Directory.Exists(objectStoragePath))
            {
                Directory.Delete(objectStoragePath, true);
            }

            // Delete the id index file
            string indexPath = GetIndexPath(objectDataKey.TypeDescriptor, id);
            if (File.Exists(indexPath))
            {
                File.Delete(indexPath);
            }

            // Delete the uuid index file if uuid exists
            string uuid = GetUuid(data.Data);
            if (!string.IsNullOrEmpty(uuid))
            {
                string uuidIndexPath = GetUuidIndexPath(objectDataKey.TypeDescriptor, uuid);
                if (File.Exists(uuidIndexPath))
                {
                    File.Delete(uuidIndexPath);
                }
            }

            return new ObjectDataDeleteResult { Success = true };
        }
        catch (Exception ex)
        {
            return new ObjectDataDeleteResult
            {
                Success = false,
                Message = ProcessMode.Current.Mode == ProcessModes.Prod
                    ? ex.GetBaseException().Message
                    : ex.GetMessageAndStackTrace()
            };
        }
    }

    private static string GetUuid(object data)
    {
        PropertyInfo uuidProp = data.GetType().GetProperty("Uuid");
        return uuidProp?.GetValue(data) as string;
    }

    private string GetIndexPath(TypeDescriptor typeDescriptor, ulong id)
    {
        List<string> parts = new List<string>();
        parts.Add(StorageManager.GetRootStorageHolder().FullName);
        parts.Add("index");
        string fullName = typeDescriptor.Type.FullName ?? "UNSPECIFIED_TYPE_NAME";
        parts.AddRange(fullName.Split('.'));
        parts.Add(id.ToString());
        return Path.Combine(parts.ToArray());
    }

    private string GetUuidIndexPath(TypeDescriptor typeDescriptor, string uuid)
    {
        List<string> parts = new List<string>();
        parts.Add(StorageManager.GetRootStorageHolder().FullName);
        parts.Add("index-uuid");
        string fullName = typeDescriptor.Type.FullName ?? "UNSPECIFIED_TYPE_NAME";
        parts.AddRange(fullName.Split('.'));
        parts.Add(uuid);
        return Path.Combine(parts.ToArray());
    }
}
