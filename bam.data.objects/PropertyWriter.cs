using Bam.Data.Dynamic.Objects;
using Bam.Data.Repositories;

namespace Bam.Data.Objects;

/// <summary>
/// Writes individual properties to storage using an <see cref="IObjectDataStorageManager"/>.
/// </summary>
public class PropertyWriter : IPropertyWriter
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyWriter"/> class.
    /// </summary>
    /// <param name="objectDataStorageManager">The storage manager used to write properties.</param>
    public PropertyWriter(IObjectDataStorageManager objectDataStorageManager)
    {
        this.ObjectDataStorageManager = objectDataStorageManager;
    }
    
    /// <summary>
    /// Gets the storage manager used for writing properties.
    /// </summary>
    public IObjectDataStorageManager ObjectDataStorageManager { get; init; }

    /// <inheritdoc />
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