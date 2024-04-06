using System.Reflection;
using Bam.Data.Objects;
using Bam.Net;
using Bam.Storage;
using Type = System.Type;

namespace Bam.Data.Dynamic.Objects;

public class ObjectStorageManager : IObjectStorageManager
{
    public ObjectStorageManager(IStorageContainer rootStorage, IObjectHashCalculator objectHashCalculator)
    {
        this.RootStorage = rootStorage;
        this.ObjectHashCalculator = objectHashCalculator;
    }
    
    public IStorageContainer RootStorage { get; private set; }
    public IObjectHashCalculator ObjectHashCalculator { get; private set; }
    
    public IStorageContainer GetRootStorageContainer()
    {
        return RootStorage;
    }

    public IStorageContainer GetTypeStorageContainer(Type type)
    {
        return new DirectoryStorageContainer(Path.Combine(GetRootStorageContainer().FullName, GetRelativePathForType(type)));
    }

    public IStorageContainer GetPropertyStorageContainer(PropertyInfo property)
    {
        return new DirectoryStorageContainer(Path.Combine(GetTypeStorageContainer(property.DeclaringType).FullName, property.Name));
    }

    public IStorageContainer GetPropertyStorageContainer(Type type, PropertyInfo property)
    {
        return new DirectoryStorageContainer(Path.Combine(GetTypeStorageContainer(type).FullName, property.Name));
    }

    public IStorageContainer GetKeyStorageContainer(IObjectKey objectKey)
    {
        IStorageIdentifier directoryInfo = GetTypeStorageContainer(objectKey.Type);
        List<string> parts = new List<string>() { directoryInfo.FullName };
        parts.Add("key");
        parts.AddRange(objectKey.Key.ToString().Split(2));
        
        return new DirectoryStorageContainer(Path.Combine(parts.ToArray()));
    }

    public IStorageContainer GetHashStorageIdentifier(IObjectIdentifier objectIdentifier)
    {
        IStorageIdentifier typeStorageIdentifier = GetTypeStorageContainer(objectIdentifier.Type);
        List<string> parts = new List<string>() { typeStorageIdentifier.FullName };
        parts.Add("hash");
        parts.AddRange(objectIdentifier.Hash.ToString().Split(2));
        return new DirectoryStorageContainer(Path.Combine(parts.ToArray()));
    }

    public virtual IStorage GetStorage(IStorageContainer storageIdentifier)
    {
        return new FsStorage(storageIdentifier.FullName);
    }

    private string GetRelativePathForType(Type type)
    {
        Args.ThrowIfNull(type, nameof(type));
        List<string> parts = new List<string>();
        parts.AddRange(type.FullName.DelimitSplit("."));
        return Path.Combine(parts.ToArray());
    }
}