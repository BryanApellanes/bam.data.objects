using Bam;
using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectDataKey : IObjectDataKey
{
    public TypeDescriptor TypeDescriptor { get; set; }
    public IStorageIdentifier StorageIdentifier { get; internal set; }
    
    public string Id { get; internal set; }
    
    public string Key { get; internal set; }

    public override string ToString()
    {
        return GetPath();
    }

    public string GetPath()
    {
        List<string> parts = new List<string>();
        parts.Add(this.StorageIdentifier.FullName);
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