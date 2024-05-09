using System.Reflection;
using Bam.Data.Dynamic.Objects;

using Bam.Net;
using Bam.Net.Data.Repositories;
using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectDataWriter : IObjectDataWriter
{
    private const string KeyFileName = "key";
    private const string DataFileName = "dat";
    
    public ObjectDataWriter(IObjectDataFactory objectDataFactory, IObjectStorageManager objectStorageManager, IObjectPropertyWriter objectPropertyWriter)
    {
        this.ObjectDataFactory = objectDataFactory;
        this.ObjectStorageManager = objectStorageManager;
        this.ObjectPropertyWriter = objectPropertyWriter;
    }
    
    public IObjectDataFactory ObjectDataFactory { get; init; }
    
    public IObjectStorageManager ObjectStorageManager { get; init; }
    
    public IObjectPropertyWriter ObjectPropertyWriter { get; init; }
    
    public Task<IObjectDataWriteResult> WriteAsync(object data)
    {
        if (data is IObjectData objectData)
        {
            return WriteAsync(objectData);
        }
        return WriteAsync(new ObjectData(data));
    }

    public async Task<IObjectDataWriteResult> WriteAsync(IObjectData data)
    {
        ObjectDataWriteResult objectDataWriteResult = new ObjectDataWriteResult(data);
        try
        {
            IObjectKey objectKey = ObjectDataFactory.GetObjectKey(data);
            IObjectIdentifier objectIdentifier = ObjectDataFactory.GetObjectIdentifier(data);

            // write the key to 
            //  {root}/objects/name/space/type/key/{K/e/y}/key -> {ObjectId}
            IStorage keyStorage = ObjectStorageManager.GetKeyStorage(objectKey);

            IStorageSlot keySlot = keyStorage.Save(KeyFileName, new RawData(objectIdentifier.Id.ToString()));
            objectDataWriteResult.ObjectKey = objectKey;
            objectDataWriteResult.KeySlot = keySlot;
            
            // write Object properties to
            // {root}/objects/name/space/type/hash/{Ob/je/ct/Id}/{propertyName}/{version}/dat content -> {RawDataHash}
            foreach (IObjectProperty property in data.Properties)
            {
                objectDataWriteResult.AddPropertyWriteResult(await ObjectPropertyWriter.WritePropertyAsync(property));
            }
        }
        catch (Exception ex)
        {
            objectDataWriteResult.Success = false;
            objectDataWriteResult.Message = ProcessMode.Current.Mode == ProcessModes.Prod ? ex.Message : ex.GetMessageAndStackTrace();
        }

        return objectDataWriteResult;
    }
}