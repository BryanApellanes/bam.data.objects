using System.Reflection;
using Bam.Data.Dynamic.Objects;

using Bam;
using Bam.Data.Repositories;
using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectDataWriter : IObjectDataWriter
{
    public ObjectDataWriter(IObjectDataFactory objectDataFactory, IObjectDataStorageManager objectDataStorageManager)
    {
        Args.ThrowIfNull(objectDataFactory, nameof(objectDataFactory));
        Args.ThrowIfNull(objectDataStorageManager, nameof(objectDataStorageManager));
        this.ObjectDataFactory = objectDataFactory;
        this.ObjectDataStorageManager = objectDataStorageManager;
    }
    
    public IObjectDataFactory ObjectDataFactory { get; init; }

    public IObjectDataLocatorFactory ObjectDataLocatorFactory => ObjectDataFactory.ObjectDataLocatorFactory;
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
            data.ObjectDataLocatorFactory ??= ObjectDataLocatorFactory;
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