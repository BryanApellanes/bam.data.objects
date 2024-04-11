using System.Reflection;
using Bam.Data.Dynamic.Objects;
using bam.storage;
using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectDataWriter : IObjectDataWriter
{
    private const string KeyFileName = "key";
    private const string DataFileName = "dat";
    
    public ObjectDataWriter(IObjectDataFactory objectDataFactory, IObjectStorageManager objectStorageManager)
    {
        this.ObjectDataFactory = objectDataFactory;
        this.ObjectStorageManager = objectStorageManager;
    }
    
    public IObjectDataFactory ObjectDataFactory { get; init; }
    
    public IObjectStorageManager ObjectStorageManager { get; init; }
    
    public Task<IObjectDataWriteResult> WriteAsync(object data)
    {
        return WriteAsync(new ObjectData(data));
    }

    public Task<IObjectDataWriteResult> WriteAsync(IObjectData data)
    {
        IObjectKey objectKey = ObjectDataFactory.GetObjectKey(data);
        IStorageContainer keyStorageIdentifier = ObjectStorageManager.GetKeyStorageContainer(objectKey);

        // write the key to 
        //  {root}/objects/name/space/type/key/{K/e/y}/key -> {HashId}
        IStorage keyStorage = ObjectStorageManager.GetStorage(keyStorageIdentifier);

        IRawData keyData = keyStorage.Save(KeyFileName, new RawData(objectKey.ToJson()));

        Type type = data.Type;
        // write Object properties to
        // {root}/objects/name/space/type/hash/{HashId}/{propertyName}/{version}/dat content -> {RawDataHash}
        foreach (IObjectProperty property in data.Properties)
        {
            IStorageContainer propertyStorage = ObjectStorageManager.GetPropertyStorageContainer(property);

            IStorage storage = ObjectStorageManager.GetStorage(propertyStorage);
            
            // Create IObjectPropertyValuePointer to save in dat
            // -> points to {RawDataHash}
            throw new NotImplementedException();
        }
        
        // write properties to Raw data
        // {root}/raw/{hash}.dat   

        throw new NotImplementedException();
    }
}