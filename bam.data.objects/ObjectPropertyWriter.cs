using Bam.Data.Dynamic.Objects;
using Bam.Net;
using Bam.Net.Data.Repositories;
using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectPropertyWriter : IObjectPropertyWriter
{
    public ObjectPropertyWriter(IObjectStorageManager objectStorageManager)
    {
        this.ObjectStorageManager = objectStorageManager;
    }
    
    public IObjectStorageManager ObjectStorageManager { get; init; }
    
    public async Task<IObjectPropertyWriteResult> WritePropertyAsync(IObjectProperty property)
    {
        try
        {
            IRawData rawPropertyData = property.ToRawData();
            IStorage rawStorage = ObjectStorageManager.GetRawStorage();
            rawStorage.Save(rawPropertyData);

            IStorageContainer propertyStorageContainer = ObjectStorageManager.GetPropertyStorageContainer(property);

            IStorage propertyStorage = ObjectStorageManager.GetStorage(propertyStorageContainer);
            propertyStorage.Save(new RawData(rawPropertyData.HashString));
            return new ObjectPropertyWriteResult
            {
                Success = true,
                ObjectProperty = property,
                RawDataHash = rawPropertyData.HashString
            };
        }
        catch (Exception ex)
        {
            return new ObjectPropertyWriteResult
            {
                Success = false,
                ObjectProperty = property,
                Message = ProcessMode.Current.Mode == ProcessModes.Prod ? ex.Message : ex.GetMessageAndStackTrace()
            };
        }

    }
}