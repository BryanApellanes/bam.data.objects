using System.Reflection;
using Bam.Data.Dynamic.Objects;
using Bam.Storage;
using Bamn.Data.Objects;

namespace Bam.Data.Objects;

public interface IObjectDataStorageManager
{
    event EventHandler<ObjectStorageEventArgs> PropertyWriteStarted;
    event EventHandler<ObjectStorageEventArgs> PropertyWriteComplete;
    event EventHandler<ObjectStorageEventArgs> PropertyWriteException;
    event EventHandler<ObjectStorageEventArgs> PropertyReadStarted;
    event EventHandler<ObjectStorageEventArgs> PropertyReadComplete;
    event EventHandler<ObjectStorageEventArgs> PropertyReadException;
    
    IRootStorageHolder GetRootStorageHolder();
    ITypeStorageHolder GetObjectStorageHolder(Type type);

    IPropertyStorageHolder GetPropertyStorageHolder(IPropertyDescriptor property);

    IStorage GetStorage();
    IStorage GetStorage(IStorageSlot slot);
    IStorage GetStorage(IStorageHolder storageIdentifier);

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
