using Bam.Data.Dynamic.Objects;
using Bam.Storage;
using Bam;

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
    
    public IPropertyStorageVersionSlot GetPropertyVersionSlot(IObjectDataStorageManager dataStorageManager, IProperty property, int version)
    {
        return dataStorageManager.GetPropertyStorageVersionSlot(property.ToDescriptor(), version);
    }

    public IPropertyWriteResult Save(IObjectDataStorageManager dataStorageManager, IProperty property)
    {
        try
        {
            if (!dataStorageManager.IsEqualToLatestVersion(property))
            {
                // find next version number
                int nextVersion = dataStorageManager.GetNextVersionNumber(property);
                // write Object properties to
                // {root}/objects/name/space/type/{Ob/je/ct/Ke/y_}/{propertyName}/{version}/dat
            
                IPropertyStorageVersionSlot slot = this.GetPropertyVersionSlot(dataStorageManager, property, nextVersion);
                return slot.Save(dataStorageManager, property);
            }
            else
            {
                return new PropertyWriteResult()
                {
                    Status = PropertyWriteResults.AlreadySaved,
                    Property = property,
                    PointerStorageSlot = this.GetPropertyVersionSlot(dataStorageManager, property, dataStorageManager.GetLatestVersionNumber(property.ToDescriptor()))
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

    public IEnumerable<IPropertyStorageVersionSlot> GetVersions(IObjectDataStorageManager dataStorageManager, IProperty property)
    {
        return dataStorageManager.GetPropertyStorageVersionSlots(property.ToDescriptor());
    }
}