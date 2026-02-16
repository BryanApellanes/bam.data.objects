using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IPropertyStorageHolder"/> that manages a directory-based storage location for a specific property.
/// </summary>
public class PropertyStorageHolder : DirectoryStorageHolder, IPropertyStorageHolder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyStorageHolder"/> class with the specified path.
    /// </summary>
    /// <param name="path">The file system path for the property storage directory.</param>
    public PropertyStorageHolder(string path) : base(path)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyStorageHolder"/> class with the specified directory.
    /// </summary>
    /// <param name="directory">The directory info for the property storage directory.</param>
    public PropertyStorageHolder(DirectoryInfo directory) : base(directory)
    {
    }

    /// <inheritdoc />
    public string PropertyName { get; internal set; }

    /// <inheritdoc />
    public ITypeStorageHolder TypeStorageHolder { get; internal set; }

    /// <inheritdoc />
    public IPropertyStorageRevisionSlot GetPropertyVersionSlot(IObjectDataStorageManager dataStorageManager, IProperty property, int version)
    {
        return dataStorageManager.GetPropertyStorageRevisionSlot(property.ToDescriptor(), version);
    }

    /// <inheritdoc />
    public IPropertyWriteResult Save(IObjectDataStorageManager dataStorageManager, IProperty property)
    {
        try
        {
            if (!dataStorageManager.IsEqualToLatestRevision(property))
            {
                // find next version number
                int nextVersion = dataStorageManager.GetNextRevisionNumber(property);
                // write Object properties to
                // {root}/objects/name/space/type/{Ob/je/ct/Ke/y_}/{propertyName}/{version}/dat
            
                IPropertyStorageRevisionSlot slot = this.GetPropertyVersionSlot(dataStorageManager, property, nextVersion);
                return slot.Save(dataStorageManager, property);
            }
            else
            {
                return new PropertyWriteResult()
                {
                    Status = PropertyWriteResults.AlreadySaved,
                    Property = property,
                    PointerStorageSlot = this.GetPropertyVersionSlot(dataStorageManager, property, dataStorageManager.GetLatestRevisionNumber(property.ToDescriptor()))
                };
            }
        }
        catch (Exception ex)
        {
            return new PropertyWriteResult()
            {
                Status = PropertyWriteResults.Failed,
                Property = property,
                Message = ProcessMode.Current.Mode == ProcessModes.Prod ? ex.Message : ex.GetMessageAndStackTrace()
            };
        }
    }

    /// <inheritdoc />
    public IEnumerable<IPropertyStorageRevisionSlot> GetVersions(IObjectDataStorageManager dataStorageManager, IProperty property)
    {
        return dataStorageManager.GetPropertyStorageVersionSlots(property.ToDescriptor());
    }
}