using System.Reflection;
using bam.data.objects;
using Bam.Data.Objects;
using Bam;
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
    
    public IRootStorageHolder GetRootStorageHolder()
    {
        return RootStorage;
    }

    public ITypeStorageHolder GetTypeStorageHolder(Type type)
    {
        IRootStorageHolder rootStorageHolder = GetRootStorageHolder();
        string relativeTypePath = GetRelativePathForType(type);
        return new TypeStorageHolder(Path.Combine(rootStorageHolder.FullName, "objects", relativeTypePath))
        {
            RootStorageHolder = this.RootStorage
        };
    }

    public IPropertyHolder GetPropertyStorageHolder(IProperty property)
    {
        List<string> parts = new List<string>();
        parts.Add(GetTypeStorageHolder(property.Parent.Type).FullName);
        parts.AddRange(property.Parent.GetObjectKey().Key.Split(2));
        parts.Add(property.PropertyName);
        return new PropertyStorageHolder(Path.Combine(parts.ToArray()))
        {
            PropertyName = property.PropertyName,
            TypeStorageHolder = GetTypeStorageHolder(property.Parent.Type)
        };
    }
    
    public virtual IStorage GetRawStorage()
    {
        return GetStorage(new DirectoryStorageHolder(Path.Combine(RootStorage.FullName, "raw")));
    }

    public bool IsSlotWritten(IStorageSlot slot)
    {
        throw new NotImplementedException();
    }

    public IPropertyStorageVersionSlot GetPropertySlot(IProperty property)
    {
        throw new NotImplementedException();
    }

    public IPropertyStorageVersionSlot GetPropertyVersionSlot(IProperty property, int version)
    {
        throw new NotImplementedException();
    }

    public int GetLatestVersionNumber(IProperty property)
    {
        throw new NotImplementedException();
    }

    public int GetNextVersionNumber(IProperty property)
    {
        throw new NotImplementedException();
    }

    public bool IsEqualToLatestVersion(IProperty property)
    {
        throw new NotImplementedException();
    }

    public bool VersionExists(IProperty property, int version = 1)
    {
        throw new NotImplementedException();
    }

    public IEnumerable<IPropertyVersion> GetVersionsAsync(IProperty property)
    {
        throw new NotImplementedException();
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