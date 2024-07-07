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
    public FsObjectStorageManager(IRootStorageHolder rootStorage, IObjectIdentityCalculator objectIdentityCalculator, IObjectLoader objectLoader)
    {
        this.RootStorage = rootStorage;
        this.ObjectIdentityCalculator = objectIdentityCalculator;
        this.ObjectLoader = objectLoader;
    }
    
    public IRootStorageHolder RootStorage { get; private set; }
    public IObjectIdentityCalculator ObjectIdentityCalculator { get; private set; }
    
    public IObjectLoader ObjectLoader { get; private set; }

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

    public ITypeStorageHolder GetObjectStorageHolder(Type type)
    {
        IRootStorageHolder rootStorageHolder = GetRootStorageHolder();
        string relativeTypePath = GetRelativePathForType(type);
        return new TypeStorageHolder(Path.Combine(rootStorageHolder.FullName, "objects", relativeTypePath))
        {
            RootStorageHolder = this.RootStorage
        };
    }

    public IPropertyStorageHolder GetPropertyStorageHolder(IPropertyDescriptor propertyDescriptor)
    {
        List<string> parts = new List<string>();
        ITypeStorageHolder typeStorageHolder = GetObjectStorageHolder(propertyDescriptor.ObjectKey.Type);
        parts.Add(typeStorageHolder.FullName);
        parts.AddRange(propertyDescriptor.ObjectKey.Key.Split(2));
        parts.Add(propertyDescriptor.PropertyName);
        return new PropertyStorageHolder(Path.Combine(parts.ToArray()))
        {
            PropertyName = propertyDescriptor.PropertyName,
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

    public IPropertyStorageVersionSlot GetLatestPropertyStorageVersionSlot(IPropertyDescriptor propertyDescriptor)
    {
        return GetPropertyStorageVersionSlot(propertyDescriptor, GetLatestVersionNumber(propertyDescriptor));
    }
    
    public IPropertyStorageVersionSlot GetNextPropertyStorageVersionSlot(IProperty property)
    {
        return GetPropertyStorageVersionSlot(property.ToDescriptor(), GetNextVersionNumber(property));
    }

    public IPropertyStorageVersionSlot GetPropertyStorageVersionSlot(IPropertyDescriptor propertyDescriptor,
        int version)
    {
        return new PropertyStorageVersionSlot(GetPropertyStorageHolder(propertyDescriptor), version);
    }
    
    public int GetLatestVersionNumber(IPropertyDescriptor property)
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
        return GetLatestVersionNumber(property.ToDescriptor()) + 1;
    }

    public bool IsEqualToLatestVersion(IProperty property)
    {
        IProperty latest = ReadProperty(property.ToDescriptor(), GetLatestPropertyStorageVersionSlot(property.ToDescriptor()));
        return latest.Decode().Equals(property.Decode());
    }

    public bool VersionExists(IPropertyDescriptor property, int version = 1)
    {
        return IsSlotWritten(GetPropertyStorageVersionSlot(property, version));
    }
    
    public IEnumerable<IPropertyStorageVersionSlot> GetVersions(IPropertyDescriptor property)
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

    public IProperty ReadProperty(IPropertyDescriptor propertyDescriptor)
    {
        return ReadProperty(propertyDescriptor, GetLatestPropertyStorageVersionSlot(propertyDescriptor));
    }
    
    public IProperty ReadProperty(IPropertyDescriptor propertyDescriptor, IStorageSlot storageSlot)
    {
        Args.ThrowIfNull(propertyDescriptor);
        Args.ThrowIfNull(storageSlot);
        
        PropertyReadStarted?.Invoke(this, new ObjectStorageEventArgs());
        IStorage pointerStorage = this.GetStorage(storageSlot);
        IRawData pointerData = pointerStorage.LoadSlot(storageSlot);
        IRawStorage rawStorage = this.GetRawStorage();
        IRawData rawData = rawStorage.LoadHashId(pointerData.Convert<ulong>());

        PropertyReadComplete?.Invoke(this, new ObjectStorageEventArgs());
        throw new NotImplementedException();
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
        string fullName = type.FullName ?? "UNSPECIFIED_TYPE_NAME";
        
        List<string> parts = new List<string>();
        parts.AddRange(fullName.DelimitSplit("."));
        return Path.Combine(parts.ToArray());
    }
}