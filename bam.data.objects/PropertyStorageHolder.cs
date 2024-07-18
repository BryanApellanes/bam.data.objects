using Bam.Data.Dynamic.Objects;
using bam.data.objects;
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
    
    public IPropertyStorageVersionSlot GetPropertyVersionSlot(IObjectStorageManager storageManager, IProperty property, int version)
    {
        return storageManager.GetPropertyStorageVersionSlot(property.ToDescriptor(), version);
    }

    public IPropertyWriteResult Save(IObjectStorageManager storageManager, IProperty property)
    {
        try
        {
            if (!storageManager.IsEqualToLatestVersion(property))
            {
                // find next version number
                int nextVersion = storageManager.GetNextVersionNumber(property);
                // write Object properties to
                // {root}/objects/name/space/type/{Ob/je/ct/Ke/y_}/{propertyName}/{version}/dat
            
                IPropertyStorageVersionSlot slot = this.GetPropertyVersionSlot(storageManager, property, nextVersion);
                return slot.Save(storageManager, property);
            }
            else
            {
                return new PropertyWriteResult()
                {
                    Status = PropertyWriteResults.AlreadySaved,
                    Property = property,
                    PointerStorageSlot = this.GetPropertyVersionSlot(storageManager, property, storageManager.GetLatestVersionNumber(property.ToDescriptor()))
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

    public IEnumerable<IPropertyStorageVersionSlot> GetVersions(IObjectStorageManager storageManager, IProperty property)
    {
        return storageManager.GetPropertyStorageVersionSlots(property.ToDescriptor());
    }
}