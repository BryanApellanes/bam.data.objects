using Bam.Data.Dynamic.Objects;
using Bam.Storage;
using Bamn.Data.Objects;

namespace Bam.Data.Objects;

public interface IObjectDataStorageManager
{
    event EventHandler<ObjectDataStorageEventArgs> PropertyWriteStarted;
    event EventHandler<ObjectDataStorageEventArgs> PropertyWriteComplete;
    event EventHandler<ObjectDataStorageEventArgs> PropertyWriteException;
    event EventHandler<ObjectDataStorageEventArgs> PropertyReadStarted;
    event EventHandler<ObjectDataStorageEventArgs> PropertyReadComplete;
    event EventHandler<ObjectDataStorageEventArgs> PropertyReadException;
    
    IRootStorageHolder GetRootStorageHolder();
    ITypeStorageHolder GetObjectStorageHolder(Type type);

    IPropertyStorageHolder GetPropertyStorageHolder(IPropertyDescriptor property);

    IObjectStorage GetStorage();
    IObjectStorage GetStorage(IStorageSlot slot);
    IObjectStorage GetStorage(IStorageHolder storageIdentifier);

    IRawStorage GetRawStorage();

    bool IsSlotWritten(IStorageSlot slot);
    IPropertyStorageVersionSlot GetLatestPropertyStorageVersionSlot(IPropertyDescriptor property);
    IPropertyStorageVersionSlot GetNextPropertyStorageVersionSlot(IProperty property);
    IPropertyStorageVersionSlot GetPropertyStorageVersionSlot(IPropertyDescriptor property, int version);
    int GetLatestVersionNumber(IPropertyDescriptor property);
    int GetNextVersionNumber(IProperty property);
    bool IsEqualToLatestVersion(IProperty property);
    bool VersionExists(IPropertyDescriptor property, int version = 1);
    IEnumerable<IPropertyVersion> ReadVersions(IObjectData parent, IPropertyDescriptor propertyDescriptor); 
    IEnumerable<IPropertyStorageVersionSlot> GetPropertyStorageVersionSlots(IPropertyDescriptor property);
    IPropertyWriteResult WriteProperty(IProperty propertyDescriptor);

    IProperty? ReadProperty(IObjectData parent, IPropertyDescriptor propertyDescriptor);

    IObjectDataWriteResult WriteObject(IObjectData data);
    IObjectData ReadObject(IObjectDataKey objectDataKey);
}
