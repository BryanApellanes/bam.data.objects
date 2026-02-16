namespace Bam.Data.Objects;

/// <summary>
/// Reads object data from storage using an <see cref="IObjectDataStorageManager"/>.
/// </summary>
public class ObjectDataReader : IObjectDataReader
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectDataReader"/> class.
    /// </summary>
    /// <param name="objectDataStorageManager">The storage manager used to read objects.</param>
    public ObjectDataReader(IObjectDataStorageManager objectDataStorageManager)
    {
        this.ObjectDataStorageManager = objectDataStorageManager;
    }
    
    /// <summary>
    /// Gets or sets the storage manager used for reading objects.
    /// </summary>
    public IObjectDataStorageManager ObjectDataStorageManager { get; set; }

    /// <inheritdoc />
    public async Task<IObjectDataReadResult> ReadObjectDataAsync(IObjectDataKey dataKey)
    {
        try
        {
            return new ObjectDataReadResult()
            {
                ObjectData = ObjectDataStorageManager.ReadObject(dataKey)
            };
        }
        catch (Exception ex)
        {
            return new ObjectDataReadResult()
            {
                Success = false,
                Message =  ProcessMode.Current.Mode == ProcessModes.Prod ? ex.Message : ex.GetMessageAndStackTrace()
            };
        }
    }
}