using System.Reflection;
using Bam.Data.Dynamic.Objects;
using bam.storage;
using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectDataWriter : IObjectDataWriter
{
    public ObjectDataWriter(IObjectIdentifierFactory objectIdentifierFactory, IObjectStorageManager objectStorageManager, IObjectHashCalculator objectHashCalculator)
    {
        this.ObjectIdentifierFactory = objectIdentifierFactory;
        this.ObjectStorageManager = objectStorageManager;
    }
    
    public IObjectIdentifierFactory ObjectIdentifierFactory { get; init; }
    public IObjectStorageManager ObjectStorageManager { get; init; }

    public IObjectHashCalculator ObjectHashCalculator { get; init; }
    
    public Task<IObjectDataWriteResult> WriteAsync(object data)
    {
        return WriteAsync(new ObjectData(data));
    }

    public Task<IObjectDataWriteResult> WriteAsync(IObjectData data)
    {
        IObjectKey objectKey = ObjectIdentifierFactory.GetObjectKeyFor(data);
        IStorageContainer keyStorageIdentifier = ObjectStorageManager.GetKeyStorageContainer(objectKey);

        // write the key to 
        //  {root}/objects/name/space/type/key/{K/e/y}/dat -> {HashId}
        IStorage keyStorage = ObjectStorageManager.GetStorage(keyStorageIdentifier);

        keyStorage.Save("dat", new RawData(objectKey.ToJson()));

        Type type = data.Type;
        // write Object properties to
        // {root}/objects/name/space/type/hash/{HashId}/{propertyName}/{version}/val.dat content -> {RawDataHash}
        foreach (IObjectProperty property in data.Properties)
        {
            PropertyInfo propertyInfo = type.GetProperty(property.PropertyName);
            IStorageContainer propertyStorage = ObjectStorageManager.GetPropertyStorageContainer(propertyInfo);

            IStorage storage = ObjectStorageManager.GetStorage(propertyStorage);
            
            // TODO: handle versioning path
            // Create IObjectPropertyValuePointer to save in val.dat
            // -> points to {RawDataHash}
            throw new NotImplementedException();
        }
        
        // write properties to Raw data
        // {root}/raw/{hash}.dat   

        throw new NotImplementedException();
    }
}