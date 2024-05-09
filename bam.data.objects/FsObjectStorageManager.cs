using System.Reflection;
using Bam.Data.Objects;
using Bam.Net;
using Bam.Storage;
using Type = System.Type;

namespace Bam.Data.Dynamic.Objects;

public class FsObjectStorageManager : IObjectStorageManager
{
    public FsObjectStorageManager(IRootStorageContainer rootStorage, IObjectHashCalculator objectHashCalculator)
    {
        this.RootStorage = rootStorage;
        this.ObjectHashCalculator = objectHashCalculator;
    }
    
    public IRootStorageContainer RootStorage { get; private set; }
    public IObjectHashCalculator ObjectHashCalculator { get; private set; }
    
    public IRootStorageContainer GetRootStorageContainer()
    {
        return RootStorage;
    }

    public IStorageContainer GetTypeStorageContainer(Type type)
    {
        IRootStorageContainer rootStorageContainer = GetRootStorageContainer();
        string relativeTypePath = GetRelativePathForType(type);
        return new DirectoryStorageContainer(Path.Combine(rootStorageContainer.FullName, "objects", relativeTypePath));
    }

    public IObjectPropertyStorageContainer GetPropertyStorageContainer(IObjectProperty property)
    {
        List<string> parts = new List<string>();
        parts.Add(GetTypeStorageContainer(property.Parent.Type).FullName);
        parts.Add(property.PropertyName);
        parts.AddRange(property.Parent.GetKey(ObjectHashCalculator).ToString().Split(2));
        return new ObjectPropertyStorageContainer(Path.Combine(parts.ToArray()));
    }

    public IStorage GetKeyStorage(IObjectKey objectKey)
    {
        return GetStorage(GetKeyStorageContainer(objectKey));
    }

    public IStorageContainer GetKeyStorageContainer(IObjectKey objectKey)
    {
        IStorageIdentifier directoryInfo = GetTypeStorageContainer(objectKey.Type.Type);
        List<string> parts = new List<string>
        {
            directoryInfo.FullName,
        };
        parts.AddRange(objectKey.Key.ToString().Split(2));
        
        return new DirectoryStorageContainer(Path.Combine(parts.ToArray()));
    }
    
    public virtual IStorage GetRawStorage()
    {
        return GetStorage(new DirectoryStorageContainer(Path.Combine(RootStorage.FullName, "raw")));
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