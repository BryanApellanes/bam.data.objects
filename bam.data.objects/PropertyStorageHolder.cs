using Bam.Data.Dynamic.Objects;
using bam.data.objects;
using Bam.Storage;
using Bam.Net;

namespace Bam.Data.Objects;

public class PropertyStorageHolder : DirectoryStorageHolder, IPropertyHolder
{
    public PropertyStorageHolder(string path) : base(path)
    {
    }

    public PropertyStorageHolder(DirectoryInfo directory) : base(directory)
    {
    }
    
    public string PropertyName { get; internal set; }
    public ITypeStorageHolder TypeStorageHolder { get; internal set; }
    
    public IPropertyStorageVersionSlot GetPropertyVersionSlot(IProperty property, int version)
    {
        return new PropertyStorageVersionSlot(this, version);
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
            
                IPropertyStorageVersionSlot slot = this.GetPropertyVersionSlot(property, nextVersion);
                return slot.Save(storageManager, property);
            }
            else
            {
                return new PropertyWriteResult()
                {
                    Status = PropertyWriteResults.AlreadySaved,
                    Property = property,
                    StorageSlot = this.GetPropertyVersionSlot(property, storageManager.GetLatestVersionNumber(property))
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
}