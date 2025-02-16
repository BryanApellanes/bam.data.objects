using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectDataKey : IObjectDataKey
{
    public TypeDescriptor TypeDescriptor { get; set; }
    

    public IStorageIdentifier GetStorageIdentifier(IObjectDataStorageManager objectDataStorageManager)
    {
        return objectDataStorageManager.GetObjectStorageHolder(TypeDescriptor);
    }
    
    public string? Key { get; init; }

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

    public override bool Equals(object? obj)
    {
        if (obj is ObjectDataKey key)
        {
            return Object.Equals(Key, key.Key);
        }

        return false;
    }

    public override int GetHashCode()
    {
        unchecked // Overflow is fine, just wrap
        {
            int hash = 17;

            if (!string.IsNullOrEmpty(Key))
            {
                hash = hash * 23 + Key.GetHashCode();
            }
            return hash;
        }
    }
}