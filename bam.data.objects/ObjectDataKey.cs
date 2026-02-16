using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataKey"/>, providing a composite-key-based identifier whose key hash is split into path segments for file system storage.
/// </summary>
public class ObjectDataKey : IObjectDataKey
{
    /// <inheritdoc />
    public TypeDescriptor TypeDescriptor { get; set; }


    /// <summary>
    /// Gets the storage identifier for this key by looking up the type storage holder from the specified storage manager.
    /// </summary>
    /// <param name="objectDataStorageManager">The storage manager to use for resolution.</param>
    /// <returns>The storage identifier.</returns>
    public IStorageIdentifier GetStorageIdentifier(IObjectDataStorageManager objectDataStorageManager)
    {
        return objectDataStorageManager.GetObjectStorageHolder(TypeDescriptor);
    }
    
    /// <inheritdoc />
    public string? Key { get; init; }

    /// <summary>
    /// Returns the resolved file system path for this key.
    /// </summary>
    /// <returns>The path string.</returns>
    public override string ToString()
    {
        return GetPath();
    }

    /// <inheritdoc />
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

    /// <inheritdoc />
    public IPropertyDescriptor Property(string propertyName)
    {
        return new PropertyDescriptor()
        {
            ObjectDataKey = this,
            PropertyName = propertyName
        };
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (obj is ObjectDataKey key)
        {
            return Object.Equals(Key, key.Key);
        }

        return false;
    }

    /// <inheritdoc />
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