using System.Reflection;
using Bam.Data.Objects;
using Bam;
using Bam.Data.Repositories;
using Bam.ExceptionHandling;
using Bam.Storage;
using Bamn.Data.Objects;
using Type = System.Type;

namespace Bam.Data.Dynamic.Objects;

public class FsObjectDataStorageManager : IObjectDataStorageManager
{
    public FsObjectDataStorageManager(IRootStorageHolder rootStorage, IObjectDataFactory objectDataFactory)
    {
        this.RootStorage = rootStorage;
        this.ObjectDataFactory = objectDataFactory;
    }

    private IRootStorageHolder RootStorage { get; }
    private IObjectDataFactory ObjectDataFactory { get; }
    private IObjectEncoderDecoder ObjectEncoderDecoder => ObjectDataFactory.ObjectEncoderDecoder;

    private IObjectDataIdentifierFactory ObjectDataIdentifierFactory => ObjectDataFactory.ObjectDataIdentifierFactory;

    public event EventHandler<ObjectDataStorageEventArgs>? PropertyWriteStarted;
    public event EventHandler<ObjectDataStorageEventArgs>? PropertyWriteComplete;
    public event EventHandler<ObjectDataStorageEventArgs>? PropertyWriteException;
    public event EventHandler<ObjectDataStorageEventArgs>? PropertyReadStarted;
    public event EventHandler<ObjectDataStorageEventArgs>? PropertyReadComplete;
    public event EventHandler<ObjectDataStorageEventArgs>? PropertyReadException;

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
        ITypeStorageHolder typeStorageHolder = GetObjectStorageHolder(propertyDescriptor.ObjectDataKey.TypeDescriptor);
        parts.Add(typeStorageHolder.FullName);
        parts.AddRange(propertyDescriptor.ObjectDataKey.Key.Split(2));
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
        List<IPropertyStorageVersionSlot> slots = GetPropertyStorageVersionSlots(property).ToList();
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
        IProperty latest = ReadProperty(property.Parent, property.ToDescriptor(), GetLatestPropertyStorageVersionSlot(property.ToDescriptor()));
        return latest.Decode().Equals(property.Decode());
    }

    public bool VersionExists(IPropertyDescriptor property, int version = 1)
    {
        return IsSlotWritten(GetPropertyStorageVersionSlot(property, version));
    }

    private void SetVersions(IProperty property)
    {
        property.Versions = ReadVersions(property.Parent, property.ToDescriptor());
    }
    
    public IEnumerable<IPropertyVersion> ReadVersions(IObjectData parent, IPropertyDescriptor propertyDescriptor)
    {
        foreach (IPropertyStorageVersionSlot propertyStorageSlot in GetPropertyStorageVersionSlots(propertyDescriptor))
        {
            yield return new PropertyVersion(ReadProperty(parent, propertyDescriptor, propertyStorageSlot), propertyStorageSlot.Version, propertyStorageSlot.MetaData);
        }
    }
    
    public IEnumerable<IPropertyStorageVersionSlot> GetPropertyStorageVersionSlots(IPropertyDescriptor property)
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
    
    private IPropertyWriteResult WriteProperty(IPropertyStorageVersionSlot versionSlot, IProperty property)
    {
        Args.ThrowIfNull(versionSlot, nameof(versionSlot));
        Args.ThrowIfNull(property, nameof(property));
        Args.ThrowIfNull(property.Parent, $"{nameof(property)}.Parent");
        
        IObjectDataKey objectDataKey = property.Parent.GetObjectKey();
        PropertyWriteResult result = new PropertyWriteResult
        {
            ObjectDataKey = objectDataKey,
            Property = property,
        };
        try
        {
            PropertyWriteStarted?.Invoke(this, new ObjectDataStorageEventArgs()
            {
                PropertyWriteResult = result
            });
            
            IStorage pointerStorage = this.GetStorage(versionSlot);
            result.PointerStorageSlot = pointerStorage.Save(property.ToRawDataPointer());

            IRawStorage rawStorage = this.GetRawStorage();
            result.ValueStorageSlot = rawStorage.Save(property.ToRawData());
            result.Status = PropertyWriteResults.Success;
            
            PropertyWriteComplete?.Invoke(this, new ObjectDataStorageEventArgs()
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
            PropertyWriteException?.Invoke(this, new ObjectDataStorageEventArgs()
            {
                PropertyWriteResult = result,
                Exception = ex
            });
        }

        return result;
    }

    public IProperty ReadProperty(IObjectData parent, IPropertyDescriptor propertyDescriptor)
    {
        IProperty property = ReadProperty(parent, propertyDescriptor, GetLatestPropertyStorageVersionSlot(propertyDescriptor));
        SetVersions(property);
        return property;
    }

    public IObjectDataWriteResult WriteObject(IObjectData data)
    {
        ObjectDataWriteResult result = new ObjectDataWriteResult(data);
        try
        {
            foreach (IProperty property in data.Properties)
            {
                result.AddPropertyWriteResult(this.WriteProperty(property));
            }
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = ProcessMode.Current.Mode == ProcessModes.Prod
                ? ex.GetBaseException().Message
                : ex.GetMessageAndStackTrace();
        }

        return result;
    }
    
    public IObjectData ReadObject(IObjectDataKey objectDataKey)
    {
        Args.ThrowIfNull(objectDataKey, nameof(objectDataKey));
        // instantiate type
        // populate type properties
        object data = objectDataKey.TypeDescriptor.Type.Construct();
        IObjectData objectData = this.ObjectDataFactory.Wrap(data);
        foreach (IProperty property in objectData.Properties)
        {
            objectData.Property(property.PropertyName, ReadProperty(objectData, property.ToDescriptor()));
        }

        return objectData;
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
    
    private IProperty? ReadProperty(IObjectData parent, IPropertyDescriptor propertyDescriptor, IStorageSlot storageSlot)
    {
        try
        {
            Args.ThrowIfNull(propertyDescriptor, nameof(propertyDescriptor));
            Args.ThrowIfNull(propertyDescriptor.ObjectDataKey, $"{nameof(propertyDescriptor)}.ObjectKey");
            Args.ThrowIfNull(propertyDescriptor.ObjectDataKey.TypeDescriptor, $"{nameof(propertyDescriptor)}.ObjectKey.Type");
            Args.ThrowIfNull(storageSlot, nameof(storageSlot));

            PropertyReadStarted?.Invoke(this,
                new ObjectDataStorageEventArgs() { PropertyDescriptor = propertyDescriptor, ReadingFrom = storageSlot });

            IStorage pointerStorage = this.GetStorage(storageSlot);
            IRawData pointerData = pointerStorage.LoadSlot(storageSlot);
            IRawStorage rawStorage = this.GetRawStorage();
            IRawData rawData = rawStorage.LoadHashHexString(pointerData.ToString());

            PropertyReadComplete?.Invoke(this,
                new ObjectDataStorageEventArgs() { PropertyDescriptor = propertyDescriptor, ReadingFrom = storageSlot });

            return this.ObjectDataFactory.PropertyFromRawData(parent, propertyDescriptor, rawData);
        }
        catch (Exception ex)
        {
            this.PropertyReadException?.Invoke(this, new ObjectDataStorageEventArgs() { Exception = ex });
        }

        return null;
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