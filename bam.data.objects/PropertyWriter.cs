using Bam.Data.Dynamic.Objects;
using Bam.Data.Repositories;

namespace Bam.Data.Objects;

public class PropertyWriter : IPropertyWriter
{
    public PropertyWriter(IObjectDataStorageManager objectDataStorageManager)
    {
        this.ObjectDataStorageManager = objectDataStorageManager;
    }
    
    public IObjectDataStorageManager ObjectDataStorageManager { get; init; }
    
    public async Task<IPropertyWriteResult> WritePropertyAsync(IProperty property)
    {
        try
        {
            IPropertyStorageHolder propertyStorageHolder = ObjectDataStorageManager.GetPropertyStorageHolder(property.ToDescriptor());

            return propertyStorageHolder.Save(ObjectDataStorageManager, property);
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