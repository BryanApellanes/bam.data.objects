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

    ISlottedStorage GetObjectStorage();
    ISlottedStorage GetObjectStorage(IStorageSlot slot);
    ISlottedStorage GetObjectStorage(IStorageHolder storageIdentifier);

    IRawStorage GetRawStorage();

    bool IsSlotWritten(IStorageSlot slot);
    IPropertyStorageRevisionSlot GetLatestPropertyStorageRevisionSlot(IPropertyDescriptor property);
    IPropertyStorageRevisionSlot GetNextPropertyStorageRevisionSlot(IProperty property);
    IPropertyStorageRevisionSlot GetPropertyStorageRevisionSlot(IPropertyDescriptor property, int version);
    int GetLatestRevisionNumber(IPropertyDescriptor property);
    int GetNextRevisionNumber(IProperty property);
    bool IsEqualToLatestRevision(IProperty property);
    bool RevisionExists(IPropertyDescriptor property, int revisionNumber = 1);
    IEnumerable<IPropertyRevision> ReadRevisions(IObjectData parent, IPropertyDescriptor propertyDescriptor); 
    IEnumerable<IPropertyStorageRevisionSlot> GetPropertyStorageVersionSlots(IPropertyDescriptor property);
    IPropertyWriteResult WriteProperty(IProperty propertyDescriptor);

    IProperty? ReadProperty(IObjectData parent, IPropertyDescriptor propertyDescriptor);

    IObjectDataWriteResult WriteObject(IObjectData data);
    IObjectData ReadObject(IObjectDataKey objectDataKey);
}
