namespace Bam.Data.Objects;

public class ObjectDataReader : IObjectDataReader
{
    public ObjectDataReader(IObjectStorageManager objectStorageManager)
    {
        this.ObjectStorageManager = objectStorageManager;
    }
    
    public IObjectStorageManager ObjectStorageManager { get; set; }
    
    public async Task<IObjectDataReadResult> ReadObjectDataAsync(IObjectKey key)
    {
        try
        {
            return new ObjectDataReadResult()
            {
                ObjectData = ObjectStorageManager.ReadObject(key)
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