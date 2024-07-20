using Bam;
using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectDataKey : IObjectDataKey
{
    public TypeDescriptor TypeDescriptor { get; set; }
    

    public IStorageIdentifier GetStorageIdentifier(IObjectDataStorageManager objectDataStorageManager)
    {
        return objectDataStorageManager.GetObjectStorageHolder(TypeDescriptor);
    }
    public string Id { get; internal set; }
    
    public string Key { get; internal set; }

    public override string ToString()
    {
        return GetPath();
    }

    public string GetPath(IObjectDataStorageManager? objectDataStorageManager = null)
    {
        List<string> parts = new List<string>();
        if (objectDataStorageManager != null)
        {
            parts.Add(this.GetStorageIdentifier(objectDataStorageManager).FullName);
        }
        if (!string.IsNullOrEmpty(this.Key))
        {
            parts.AddRange(this.Key.Split(2));
        }
        else if (!string.IsNullOrEmpty(this.Id))
        {
            parts.AddRange(this.Id.Split(2));
        }

        return Path.Combine(parts.ToArray());
    }

    public IPropertyDescriptor Property(string propertyName)
    {
        return new PropertyDescriptor()
        {
            ObjectDataKey = this,
            PropertyName = propertyName
        };
    }
}