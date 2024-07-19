using System.Reflection;
using Bam.Data.Dynamic.Objects;

using Bam;
using Bam.Data.Repositories;
using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectDataWriter : IObjectDataWriter
{
    public ObjectDataWriter(IObjectDataFactory objectDataFactory, IObjectIdentifierFactory objectIdentifierFactory, IObjectStorageManager objectStorageManager)
    {
        this.ObjectDataFactory = objectDataFactory;
        this.ObjectIdentifierFactory = objectIdentifierFactory;
        this.ObjectStorageManager = objectStorageManager;
    }
    
    public IObjectDataFactory ObjectDataFactory { get; init; }
    public IObjectIdentifierFactory ObjectIdentifierFactory { get; init; }
    public IObjectStorageManager ObjectStorageManager { get; init; }
    
    public Task<IObjectDataWriteResult> WriteAsync(object data)
    {
        if (data is IObjectData objectData)
        {
            return WriteAsync(objectData);
        }
        return WriteAsync(ObjectDataFactory.Wrap(data));
    }

    public async Task<IObjectDataWriteResult> WriteAsync(IObjectData data)
    {
        ObjectDataWriteResult objectDataWriteResult = new ObjectDataWriteResult(data);
        try
        {
            data.ObjectIdentifierFactory ??= ObjectIdentifierFactory;
            return ObjectStorageManager.WriteObject(data);
        }
        catch (Exception ex)
        {
            objectDataWriteResult.Success = false;
            objectDataWriteResult.Message = ProcessMode.Current.Mode == ProcessModes.Prod ? ex.Message : ex.GetMessageAndStackTrace();
        }

        return objectDataWriteResult;
    }
}