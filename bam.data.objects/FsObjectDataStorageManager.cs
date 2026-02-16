using System.Reflection;
using Bam.Data.Objects;
using Bam.Storage;
using Bamn.Data.Objects;
using Type = System.Type;

namespace Bam.Data.Dynamic.Objects;

/// <summary>
/// File-system-based implementation of <see cref="IObjectDataStorageManager"/> that stores object properties as versioned files in a directory tree.
/// </summary>
public class FsObjectDataStorageManager : IObjectDataStorageManager
{
    /// <summary>
    /// Initializes a new instance of the <see cref="FsObjectDataStorageManager"/> class.
    /// </summary>
    /// <param name="rootStorage">The root storage holder for the file system storage location.</param>
    /// <param name="objectDataFactory">The factory used to create object data wrappers and encode/decode properties.</param>
    public FsObjectDataStorageManager(IRootStorageHolder rootStorage, IObjectDataFactory objectDataFactory)
    {
        this.RootStorage = rootStorage;
        this.ObjectDataFactory = objectDataFactory;
    }

    private IRootStorageHolder RootStorage { get; }
    private IObjectDataFactory ObjectDataFactory { get; }
    private IObjectEncoderDecoder ObjectEncoderDecoder => ObjectDataFactory.ObjectEncoderDecoder;

    private IObjectDataLocatorFactory ObjectDataLocatorFactory => ObjectDataFactory.ObjectDataLocatorFactory;

    /// <inheritdoc />
    public event EventHandler<ObjectDataStorageEventArgs>? PropertyWriteStarted;

    /// <inheritdoc />
    public event EventHandler<ObjectDataStorageEventArgs>? PropertyWriteComplete;

    /// <inheritdoc />
    public event EventHandler<ObjectDataStorageEventArgs>? PropertyWriteException;

    /// <inheritdoc />
    public event EventHandler<ObjectDataStorageEventArgs>? PropertyReadStarted;

    /// <inheritdoc />
    public event EventHandler<ObjectDataStorageEventArgs>? PropertyReadComplete;

    /// <inheritdoc />
    public event EventHandler<ObjectDataStorageEventArgs>? PropertyReadException;

    /// <inheritdoc />
    public IRootStorageHolder GetRootStorageHolder()
    {
        return RootStorage;
    }

    /// <inheritdoc />
    public ITypeStorageHolder GetObjectStorageHolder(Type type)
    {
        IRootStorageHolder rootStorageHolder = GetRootStorageHolder();
        string relativeTypePath = GetRelativePathForType(type);
        return new TypeStorageHolder(Path.Combine(rootStorageHolder.FullName, "objects", relativeTypePath))
        {
            RootStorageHolder = this.RootStorage
        };
    }

    /// <inheritdoc />
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
    
    /// <inheritdoc />
    public virtual IRawStorage GetRawStorage()
    {
        return GetObjectStorage(new DirectoryStorageHolder(Path.Combine(RootStorage.FullName, "raw")));
    }

    /// <inheritdoc />
    public bool IsSlotWritten(IStorageSlot slot)
    {
        return File.Exists(slot.FullName);
    }

    /// <inheritdoc />
    public IPropertyStorageRevisionSlot GetLatestPropertyStorageRevisionSlot(IPropertyDescriptor propertyDescriptor)
    {
        return GetPropertyStorageRevisionSlot(propertyDescriptor, GetLatestRevisionNumber(propertyDescriptor));
    }
    
    /// <inheritdoc />
    public IPropertyStorageRevisionSlot GetNextPropertyStorageRevisionSlot(IProperty property)
    {
        return GetPropertyStorageRevisionSlot(property.ToDescriptor(), GetNextRevisionNumber(property));
    }

    /// <inheritdoc />
    public IPropertyStorageRevisionSlot GetPropertyStorageRevisionSlot(IPropertyDescriptor propertyDescriptor,
        int version)
    {
        return new PropertyStorageRevisionSlot(GetPropertyStorageHolder(propertyDescriptor), version);
    }
    
    /// <inheritdoc />
    public int GetLatestRevisionNumber(IPropertyDescriptor property)
    {
        List<IPropertyStorageRevisionSlot> slots = GetPropertyStorageVersionSlots(property).ToList();
        if (slots.Any())
        {
            return slots[^1].Revision;
        }

        return 0;
    }

    /// <inheritdoc />
    public int GetNextRevisionNumber(IProperty property)
    {
        return GetLatestRevisionNumber(property.ToDescriptor()) + 1;
    }

    /// <inheritdoc />
    public bool IsEqualToLatestRevision(IProperty property)
    {
        int latestRevision = GetLatestRevisionNumber(property.ToDescriptor());
        if (latestRevision == 0)
        {
            return false;
        }
        IProperty latest = ReadProperty(property.Parent, property.ToDescriptor(), GetLatestPropertyStorageRevisionSlot(property.ToDescriptor()));
        return latest != null && latest.Decode().Equals(property.Decode());
    }

    /// <inheritdoc />
    public bool RevisionExists(IPropertyDescriptor property, int revisionNumber = 1)
    {
        return IsSlotWritten(GetPropertyStorageRevisionSlot(property, revisionNumber));
    }

    private void SetVersions(IProperty property)
    {
        property.Versions = ReadRevisions(property.Parent, property.ToDescriptor());
    }
    
    /// <inheritdoc />
    public IEnumerable<IPropertyRevision> ReadRevisions(IObjectData parent, IPropertyDescriptor propertyDescriptor)
    {
        foreach (IPropertyStorageRevisionSlot propertyStorageSlot in GetPropertyStorageVersionSlots(propertyDescriptor))
        {
            yield return new PropertyRevision(ReadProperty(parent, propertyDescriptor, propertyStorageSlot), propertyStorageSlot.Revision, propertyStorageSlot.MetaData);
        }
    }
    
    /// <inheritdoc />
    public IEnumerable<IPropertyStorageRevisionSlot> GetPropertyStorageVersionSlots(IPropertyDescriptor property)
    {
        int number = 1;

        IPropertyStorageRevisionSlot slot = GetPropertyStorageRevisionSlot(property, number);
        while (IsSlotWritten(slot))
        {
            yield return slot;
            number++;
            slot = GetPropertyStorageRevisionSlot(property, number);
        }
    }

    /// <inheritdoc />
    public IPropertyWriteResult WriteProperty(IProperty property)
    {
        IPropertyStorageRevisionSlot slot = GetNextPropertyStorageRevisionSlot(property);
        return WriteProperty(slot, property);
    }
    
    private IPropertyWriteResult WriteProperty(IPropertyStorageRevisionSlot revisionSlot, IProperty property)
    {
        Args.ThrowIfNull(revisionSlot, nameof(revisionSlot));
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
            
            ISlottedStorage pointerSlottedStorage = this.GetObjectStorage(revisionSlot);
            result.PointerStorageSlot = pointerSlottedStorage.Save(property.ToRawDataPointer());

            IRawData rawData = property.ToRawData();
            IRawStorage rawStorage = this.GetRawStorage();
            result.ValueStorageSlot = rawStorage.Save(rawData);
            result.RawData = rawData;
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

    /// <inheritdoc />
    public IProperty ReadProperty(IObjectData parent, IPropertyDescriptor propertyDescriptor)
    {
        IProperty property = ReadProperty(parent, propertyDescriptor, GetLatestPropertyStorageRevisionSlot(propertyDescriptor));
        SetVersions(property);
        return property;
    }

    /// <inheritdoc />
    public IObjectDataWriteResult WriteObject(IObjectData data)
    {
        ObjectDataWriteResult result = new ObjectDataWriteResult(data);
        try
        {
            foreach (IProperty property in data.Properties)
            {
                result.AddPropertyWriteResult(this.WriteProperty(property));
            }

            IObjectDataKey objectDataKey = data.GetObjectKey();
            ITypeStorageHolder typeHolder = GetObjectStorageHolder(objectDataKey.TypeDescriptor);
            List<string> keyParts = new List<string> { typeHolder.FullName };
            keyParts.AddRange(objectDataKey.Key.Split(2));
            result.KeySlot = new FsStorageSlot(new DirectoryStorageHolder(Path.Combine(keyParts.ToArray())), "key");
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
    
    /// <inheritdoc />
    public IObjectData ReadObject(IObjectDataKey objectDataKey)
    {
        Args.ThrowIfNull(objectDataKey, nameof(objectDataKey));
        Type type = objectDataKey.TypeDescriptor.Type;
        object data = type.Construct();
        IObjectData objectData = this.ObjectDataFactory.GetObjectData(data);
        IDataTypeTranslator translator = Bam.Data.DataTypeTranslator.Default;

        foreach (PropertyInfo propertyInfo in type.GetProperties())
        {
            DataTypes enumType = translator.EnumFromType(propertyInfo.PropertyType);
            if (enumType == DataTypes.Default) continue;

            IPropertyDescriptor descriptor = new PropertyDescriptor()
            {
                ObjectDataKey = objectDataKey,
                PropertyName = propertyInfo.Name,
                PropertyType = propertyInfo.PropertyType
            };

            if (GetLatestRevisionNumber(descriptor) == 0) continue;

            IProperty readProperty = ReadProperty(objectData, descriptor);
            if (readProperty != null)
            {
                propertyInfo.SetValue(data, readProperty.Decode());
            }
        }

        return objectData;
    }

    /// <inheritdoc />
    public virtual ISlottedStorage GetObjectStorage()
    {
        return GetObjectStorage(this.RootStorage);
    }

    /// <inheritdoc />
    public ISlottedStorage GetObjectStorage(IStorageSlot slot)
    {
        ISlottedStorage slottedStorage = GetObjectStorage(slot.StorageHolder);
        slottedStorage.CurrentSlot = slot;
        return slottedStorage;
    }

    /// <inheritdoc />
    public virtual ISlottedStorage GetObjectStorage(IStorageHolder storageIdentifier)
    {
        return new FsSlottedStorage(storageIdentifier.FullName);
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

            ISlottedStorage pointerSlottedStorage = this.GetObjectStorage(storageSlot);
            IRawData pointerData = pointerSlottedStorage.LoadSlot(storageSlot);
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