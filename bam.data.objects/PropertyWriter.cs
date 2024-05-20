using System.Reflection;
using Bam.Data.Dynamic.Objects;
using Bam.Net;
using Bam.Net.Data.Repositories;
using Bam.Storage;

namespace Bam.Data.Objects;

public class PropertyWriter : IPropertyWriter
{
    public PropertyWriter(IObjectStorageManager objectStorageManager)
    {
        this.ObjectStorageManager = objectStorageManager;
    }
    
    public IObjectStorageManager ObjectStorageManager { get; init; }
    
    public async Task<IPropertyWriteResult> WritePropertyAsync(IProperty property)
    {
        try
        {
            IPropertyHolder propertyHolder = ObjectStorageManager.GetPropertyStorageHolder(property);

            return propertyHolder.Save(ObjectStorageManager, property);
        }
        catch (Exception ex)
        {
            return new PropertyWriteResult
            {
                Status = PropertyWriteResults.Failed,
                Property = property,
                Message = ProcessMode.Current.Mode == ProcessModes.Prod ? ex.Message : ex.GetMessageAndStackTrace()
            };
        }

    }

}