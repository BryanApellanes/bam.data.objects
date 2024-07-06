using System.Reflection;
using Bam.Data.Dynamic.Objects;
using bam.data.objects;
using Bam.Storage;
using Bamn.Data.Objects;

namespace Bam.Data.Objects;

public interface IObjectStorageManager
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
    IEnumerable<IPropertyStorageVersionSlot> GetVersions(IPropertyDescriptor property);
    IPropertyWriteResult WriteProperty(IProperty property);
    
    IPropertyWriteResult WriteProperty(IPropertyStorageVersionSlot versionSlot, IProperty property);
    IProperty ReadProperty(IPropertyDescriptor propertyDescriptor);
    IProperty ReadProperty(IPropertyDescriptor propertyDescriptor, IStorageSlot storageSlot);
    IProperty ReadObject(IObjectKey objectKey);
}
