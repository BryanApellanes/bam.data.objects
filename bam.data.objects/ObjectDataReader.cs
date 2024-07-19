namespace Bam.Data.Objects;

public class ObjectDataReader : IObjectDataReader
{
    public ObjectDataReader(IObjectDataStorageManager objectDataStorageManager)
    {
        this.ObjectDataStorageManager = objectDataStorageManager;
    }
    
    public IObjectDataStorageManager ObjectDataStorageManager { get; set; }
    
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