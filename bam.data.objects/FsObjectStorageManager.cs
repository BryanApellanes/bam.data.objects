using System.Reflection;
using Bam.Data.Objects;
using Bam.Net;
using Bam.Storage;
using Type = System.Type;

namespace Bam.Data.Dynamic.Objects;

public class FsObjectStorageManager : IObjectStorageManager
{
    public FsObjectStorageManager(IRootStorageHolder rootStorage, IObjectCalculator objectCalculator)
    {
        this.RootStorage = rootStorage;
        this.ObjectCalculator = objectCalculator;
    }
    
    public IRootStorageHolder RootStorage { get; private set; }
    public IObjectCalculator ObjectCalculator { get; private set; }
    
    public IRootStorageHolder GetRootStorageContainer()
    {
        return RootStorage;
    }

    public IStorageHolder GetTypeStorageContainer(Type type)
    {
        IRootStorageHolder rootStorageHolder = GetRootStorageContainer();
        string relativeTypePath = GetRelativePathForType(type);
        return new DirectoryStorageHolder(Path.Combine(rootStorageHolder.FullName, "objects", relativeTypePath));
    }

    public IObjectPropertyStorageHolder GetPropertyStorageContainer(IObjectProperty property)
    {
        List<string> parts = new List<string>();
        parts.Add(GetTypeStorageContainer(property.Parent.Type).FullName);
        parts.Add(property.PropertyName);
        parts.AddRange(ObjectCalculator.CalculateULongKey(property.Parent).ToString().Split(2));
        return new ObjectPropertyStorageHolder(Path.Combine(parts.ToArray()));
    }

    public IStorage GetKeyStorage(IObjectKey objectKey)
    {
        return GetStorage(GetKeyStorageContainer(objectKey));
    }

    public IStorageHolder GetKeyStorageContainer(IObjectKey objectKey)
    {
        IStorageIdentifier directoryInfo = GetTypeStorageContainer(objectKey.Type.Type);
        List<string> parts = new List<string>
        {
            directoryInfo.FullName,
        };
        parts.AddRange(objectKey.Key.ToString().Split(2));
        
        return new DirectoryStorageHolder(Path.Combine(parts.ToArray()));
    }
    
    public virtual IStorage GetRawStorage()
    {
        return GetStorage(new DirectoryStorageHolder(Path.Combine(RootStorage.FullName, "raw")));
    }
    

    public virtual IStorage GetStorage(IStorageHolder storageIdentifier)
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