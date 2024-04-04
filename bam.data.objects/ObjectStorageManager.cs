using System.Reflection;
using Bam.Net;
using Bam.Storage;
using Type = System.Type;

namespace Bam.Data.Dynamic.Objects;

public class ObjectStorageManager : IObjectStorageManager
{
    public ObjectStorageManager(FsStorageIdentifier rootDirectory, IObjectHashCalculator objectHashCalculator)
    {
        this.RootDirectory = rootDirectory;
        this.ObjectHashCalculator = objectHashCalculator;
    }
    
    public FsStorageIdentifier RootDirectory { get; private set; }
    public IObjectHashCalculator ObjectHashCalculator { get; private set; }
    
    public IStorageIdentifier GetRootStorage()
    {
        return RootDirectory;
    }

    public IStorageIdentifier GetTypeStorage(Type type)
    {
        return new FsStorageIdentifier(Path.Combine(GetRootStorage().Value, GetRelativePathForType(type)));
    }

    public IStorageIdentifier GetPropertyStorage(PropertyInfo property)
    {
        return new FsStorageIdentifier(Path.Combine(GetTypeStorage(property.DeclaringType).Value, property.Name));
    }

    public IStorageIdentifier GetPropertyStorage(Type type, PropertyInfo property)
    {
        return new FsStorageIdentifier(Path.Combine(GetTypeStorage(type).Value, property.Name));
    }

    public IStorageIdentifier GetKeyStorage(PropertyInfo property)
    {
        return GetKeyStorage(property.DeclaringType, property);
    }

    public IStorageIdentifier GetKeyStorage(Type type, PropertyInfo property)
    {
        IStorageIdentifier directoryInfo = GetPropertyStorage(type, property);
        return new FsStorageIdentifier(Path.Combine(directoryInfo.Value, "keys"));
    }

    public IStorageIdentifier GetHashStorage(PropertyInfo property)
    {
        return GetHashStorage(property.DeclaringType, property);
    }

    public IStorageIdentifier GetHashStorage(Type type, PropertyInfo property)
    {
        IStorageIdentifier directoryInfo = GetPropertyStorage(type, property);
        return new FsStorageIdentifier(Path.Combine(directoryInfo.Value, "hashes"));
    }

    public IStorage GetStorage(IStorageIdentifier storageIdentifier)
    {
        return new FsStorage(storageIdentifier.Value);
    }

    private string GetRelativePathForType(Type type)
    {
        Args.ThrowIfNull(type, nameof(type));
        List<string> parts = new List<string>();
        parts.AddRange(type.FullName.DelimitSplit("."));
        return Path.Combine(parts.ToArray());
    }
}