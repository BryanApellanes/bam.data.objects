using System.Reflection;
using Bam.Data.Dynamic.Objects;
using Bam;
using Bam.Data.Repositories;
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
            IPropertyStorageHolder propertyStorageHolder = ObjectStorageManager.GetPropertyStorageHolder(property.ToDescriptor());

            return propertyStorageHolder.Save(ObjectStorageManager, property);
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