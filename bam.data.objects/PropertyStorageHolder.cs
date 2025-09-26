using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public class PropertyStorageHolder : DirectoryStorageHolder, IPropertyStorageHolder
{
    public PropertyStorageHolder(string path) : base(path)
    {
    }

    public PropertyStorageHolder(DirectoryInfo directory) : base(directory)
    {
    }
    
    public string PropertyName { get; internal set; }
    public ITypeStorageHolder TypeStorageHolder { get; internal set; }
    
    public IPropertyStorageRevisionSlot GetPropertyVersionSlot(IObjectDataStorageManager dataStorageManager, IProperty property, int version)
    {
        return dataStorageManager.GetPropertyStorageRevisionSlot(property.ToDescriptor(), version);
    }

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

    public IEnumerable<IPropertyStorageRevisionSlot> GetVersions(IObjectDataStorageManager dataStorageManager, IProperty property)
    {
        return dataStorageManager.GetPropertyStorageVersionSlots(property.ToDescriptor());
    }
}