namespace Bam.Data.Objects;

/// <summary>
/// Writes object data to storage using an <see cref="IObjectDataFactory"/> and <see cref="IObjectDataStorageManager"/>.
/// </summary>
public class ObjectDataWriter : IObjectDataWriter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectDataWriter"/> class.
    /// </summary>
    /// <param name="objectDataFactory">The factory used to wrap objects in <see cref="IObjectData"/>.</param>
    /// <param name="objectDataStorageManager">The storage manager used to write objects.</param>
    public ObjectDataWriter(IObjectDataFactory objectDataFactory, IObjectDataStorageManager objectDataStorageManager)
    {
        Args.ThrowIfNull(objectDataFactory, nameof(objectDataFactory));
        Args.ThrowIfNull(objectDataStorageManager, nameof(objectDataStorageManager));
        this.ObjectDataFactory = objectDataFactory;
        this.ObjectDataStorageManager = objectDataStorageManager;
    }
    
    /// <summary>
    /// Gets the factory used to wrap objects in <see cref="IObjectData"/>.
    /// </summary>
    public IObjectDataFactory ObjectDataFactory { get; init; }

    /// <summary>
    /// Gets the locator factory from the object data factory, used to compute keys and identifiers.
    /// </summary>
    public IObjectDataLocatorFactory ObjectDataLocatorFactory => ObjectDataFactory.ObjectDataLocatorFactory;

    /// <summary>
    /// Gets the storage manager used to persist object data.
    /// </summary>
    public IObjectDataStorageManager ObjectDataStorageManager { get; init; }

    /// <inheritdoc />
    public Task<IObjectDataWriteResult> WriteAsync(object data)
    {
        if (data is IObjectData objectData)
        {
            return WriteAsync(objectData);
        }
        return WriteAsync(ObjectDataFactory.GetObjectData(data));
    }

    /// <inheritdoc />
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