using System.Reflection;
using Bam.Data.Dynamic.Objects;

using Bam;
using Bam.Data.Repositories;
using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectDataWriter : IObjectDataWriter
{
    public ObjectDataWriter(IObjectDataFactory objectDataFactory, IObjectDataIdentifierFactory objectDataIdentifierFactory, IObjectDataStorageManager objectDataStorageManager)
    {
        this.ObjectDataFactory = objectDataFactory;
        this.ObjectDataIdentifierFactory = objectDataIdentifierFactory;
        this.ObjectDataStorageManager = objectDataStorageManager;
    }
    
    public IObjectDataFactory ObjectDataFactory { get; init; }
    public IObjectDataIdentifierFactory ObjectDataIdentifierFactory { get; init; }
    public IObjectDataStorageManager ObjectDataStorageManager { get; init; }
    
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
            data.ObjectDataIdentifierFactory ??= ObjectDataIdentifierFactory;
            return ObjectDataStorageManager.WriteObject(data);
        }
        catch (Exception ex)
        {
            objectDataWriteResult.Success = false;
            objectDataWriteResult.Message = ProcessMode.Current.Mode == ProcessModes.Prod ? ex.Message : ex.GetMessageAndStackTrace();
        }

        return objectDataWriteResult;
    }
}