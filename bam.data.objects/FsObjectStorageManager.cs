using System.Reflection;
using bam.data.objects;
using Bam.Data.Objects;
using Bam;
using Bam.ExceptionHandling;
using Bam.Storage;
using Bamn.Data.Objects;
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

    public event EventHandler<ObjectStorageEventArgs>? PropertyWriteStarted;
    public event EventHandler<ObjectStorageEventArgs>? PropertyWriteComplete;
    public event EventHandler<ObjectStorageEventArgs>? PropertyWriteException;
    public event EventHandler<ObjectStorageEventArgs>? PropertyReadStarted;
    public event EventHandler<ObjectStorageEventArgs>? PropertyReadComplete;
    public event EventHandler<ObjectStorageEventArgs>? PropertyReadException;

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

    public IPropertyStorageHolder GetPropertyStorageHolder(IProperty property)
    {
        List<string> parts = new List<string>();
        ITypeStorageHolder typeStorageHolder = GetTypeStorageHolder(property.Parent.Type);
        parts.Add(typeStorageHolder.FullName);
        parts.AddRange(property.Parent.GetObjectKey().Key.Split(2));
        parts.Add(property.PropertyName);
        return new PropertyStorageHolder(Path.Combine(parts.ToArray()))
        {
            PropertyName = property.PropertyName,
            TypeStorageHolder = typeStorageHolder
        };
    }
    
    public virtual IRawStorage GetRawStorage()
    {
        return GetStorage(new DirectoryStorageHolder(Path.Combine(RootStorage.FullName, "raw")));
    }

    public bool IsSlotWritten(IStorageSlot slot)
    {
        return File.Exists(slot.FullName);
    }

    public IPropertyStorageVersionSlot GetLatestPropertyStorageVersionSlot(IProperty property)
    {
        return GetPropertyStorageVersionSlot(property, GetLatestVersionNumber(property));
    }

    public IPropertyStorageVersionSlot GetNextPropertyStorageVersionSlot(IProperty property)
    {
        return GetPropertyStorageVersionSlot(property, GetNextVersionNumber(property));
    }

    public IPropertyStorageVersionSlot GetPropertyStorageVersionSlot(IProperty property, int version)
    {
        return new PropertyStorageVersionSlot(GetPropertyStorageHolder(property), version);
    }

    public int GetLatestVersionNumber(IProperty property)
    {
        List<IPropertyStorageVersionSlot> slots = GetVersions(property).ToList();
        if (slots.Any())
        {
            return slots[^1].Version;
        }

        return 0;
    }

    public int GetNextVersionNumber(IProperty property)
    {
        return GetLatestVersionNumber(property) + 1;
    }

    public bool IsEqualToLatestVersion(IProperty property)
    {
        IProperty latest = ReadProperty(GetLatestPropertyStorageVersionSlot(property));
        return latest.Decode().Equals(property.Decode());
    }

    public bool VersionExists(IProperty property, int version = 1)
    {
        return IsSlotWritten(GetPropertyStorageVersionSlot(property, version));
    }

    public IEnumerable<IPropertyStorageVersionSlot> GetVersions(IProperty property)
    {
        int number = 1;

        IPropertyStorageVersionSlot slot = GetPropertyStorageVersionSlot(property, number);
        while (IsSlotWritten(slot))
        {
            yield return slot;
            number++;
            slot = GetPropertyStorageVersionSlot(property, number);
        }
    }

    public IPropertyWriteResult WriteProperty(IProperty property)
    {
        IPropertyStorageVersionSlot slot = GetNextPropertyStorageVersionSlot(property);
        return WriteProperty(slot, property);
    }
    
    public IPropertyWriteResult WriteProperty(IPropertyStorageVersionSlot versionSlot, IProperty property)
    {
        Args.ThrowIfNull(versionSlot, nameof(versionSlot));
        Args.ThrowIfNull(property, nameof(property));
        Args.ThrowIfNull(property.Parent, $"{nameof(property)}.Parent");
        
        IObjectKey objectKey = property.Parent.GetObjectKey();
        PropertyWriteResult result = new PropertyWriteResult
        {
            ObjectKey = objectKey,
            Property = property,
        };
        try
        {
            PropertyWriteStarted?.Invoke(this, new ObjectStorageEventArgs()
            {
                PropertyWriteResult = result
            });
            
            IStorage pointerStorage = this.GetStorage(versionSlot);
            result.PointerStorageSlot = pointerStorage.Save(property.ToRawDataPointer());

            IRawStorage rawStorage = this.GetRawStorage();
            result.ValueStorageSlot = rawStorage.Save(property.ToRawData());
            result.Status = PropertyWriteResults.Success;
            
            PropertyWriteComplete?.Invoke(this, new ObjectStorageEventArgs()
            {
                PropertyWriteResult = result
            });
        }
        catch (Exception ex)
        {
            result.Message = ProcessMode.Current.Mode == ProcessModes.Prod
                ? ex.GetBaseException().Message
                : ex.GetMessageAndStackTrace();
            result.Status = PropertyWriteResults.Failed;
            PropertyWriteException?.Invoke(this, new ObjectStorageEventArgs()
            {
                PropertyWriteResult = result,
                Exception = ex
            });
        }

        return result;
    }

    public IProperty ReadProperty(IPropertyStorageVersionSlot versionSlot)
    {
        throw new NotImplementedException();
        Args.ThrowIfNull(versionSlot);
        
        PropertyReadStarted?.Invoke(this, new ObjectStorageEventArgs());
        IStorage pointerStorage = this.GetStorage(versionSlot);
        IRawData pointerData =pointerStorage.Load(versionSlot);


        PropertyReadComplete?.Invoke(this, new ObjectStorageEventArgs());
    }

    public IProperty ReadObject(IObjectKey objectKey)
    {
        throw new NotImplementedException();
        Args.ThrowIfNull(objectKey, nameof(objectKey));
        
        /*try
        {
            IStorage pointerStorage = this.GetStorage(propertyStorageVersionSlot);
            IRawData rawPointerData = pointerStorage.Load();

            IStorage rawStorage = this.GetRawStorage();
            rawStorage.Load();
        }
        catch (Exception ex)
        {
            
        }*/
    }

    public virtual IStorage GetStorage()
    {
        return GetStorage(this.RootStorage);
    }

    public IStorage GetStorage(IStorageSlot slot)
    {
        IStorage storage = GetStorage(slot.StorageHolder);
        storage.CurrentSlot = slot;
        return storage;
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