using Bam.Data.Dynamic.Objects;
using Bam.Storage;
using Bamn.Data.Objects;

namespace Bam.Data.Objects;

/// <summary>
/// Manages the storage of object data and properties, including versioned property storage, reading, writing, and revision tracking.
/// </summary>
public interface IObjectDataStorageManager
{
    /// <summary>
    /// Occurs when a property write operation begins.
    /// </summary>
    event EventHandler<ObjectDataStorageEventArgs> PropertyWriteStarted;

    /// <summary>
    /// Occurs when a property write operation completes successfully.
    /// </summary>
    event EventHandler<ObjectDataStorageEventArgs> PropertyWriteComplete;

    /// <summary>
    /// Occurs when a property write operation encounters an exception.
    /// </summary>
    event EventHandler<ObjectDataStorageEventArgs> PropertyWriteException;

    /// <summary>
    /// Occurs when a property read operation begins.
    /// </summary>
    event EventHandler<ObjectDataStorageEventArgs> PropertyReadStarted;

    /// <summary>
    /// Occurs when a property read operation completes successfully.
    /// </summary>
    event EventHandler<ObjectDataStorageEventArgs> PropertyReadComplete;

    /// <summary>
    /// Occurs when a property read operation encounters an exception.
    /// </summary>
    event EventHandler<ObjectDataStorageEventArgs> PropertyReadException;

    /// <summary>
    /// Gets the root storage holder for this storage manager.
    /// </summary>
    /// <returns>The root storage holder.</returns>
    IRootStorageHolder GetRootStorageHolder();

    /// <summary>
    /// Gets the type-scoped storage holder for the specified type, located under the root storage.
    /// </summary>
    /// <param name="type">The type to get storage for.</param>
    /// <returns>The type storage holder.</returns>
    ITypeStorageHolder GetObjectStorageHolder(Type type);

    /// <summary>
    /// Gets the property storage holder for the specified property descriptor, located under the type and key path.
    /// </summary>
    /// <param name="property">The property descriptor identifying the property.</param>
    /// <returns>The property storage holder.</returns>
    IPropertyStorageHolder GetPropertyStorageHolder(IPropertyDescriptor property);

    /// <summary>
    /// Gets the slotted storage rooted at the root storage holder.
    /// </summary>
    /// <returns>The slotted storage instance.</returns>
    ISlottedStorage GetObjectStorage();

    /// <summary>
    /// Gets the slotted storage for the specified storage slot, with the current slot set.
    /// </summary>
    /// <param name="slot">The storage slot to scope the storage to.</param>
    /// <returns>The slotted storage instance.</returns>
    ISlottedStorage GetObjectStorage(IStorageSlot slot);

    /// <summary>
    /// Gets the slotted storage for the specified storage holder.
    /// </summary>
    /// <param name="storageIdentifier">The storage holder to scope the storage to.</param>
    /// <returns>The slotted storage instance.</returns>
    ISlottedStorage GetObjectStorage(IStorageHolder storageIdentifier);

    /// <summary>
    /// Gets the raw data storage used for content-addressed deduplication.
    /// </summary>
    /// <returns>The raw storage instance.</returns>
    IRawStorage GetRawStorage();

    /// <summary>
    /// Determines whether data has been written to the specified storage slot.
    /// </summary>
    /// <param name="slot">The storage slot to check.</param>
    /// <returns>True if the slot contains data; otherwise, false.</returns>
    bool IsSlotWritten(IStorageSlot slot);

    /// <summary>
    /// Gets the storage revision slot for the latest revision of the specified property.
    /// </summary>
    /// <param name="property">The property descriptor to look up.</param>
    /// <returns>The latest revision slot.</returns>
    IPropertyStorageRevisionSlot GetLatestPropertyStorageRevisionSlot(IPropertyDescriptor property);

    /// <summary>
    /// Gets the storage revision slot for the next revision of the specified property.
    /// </summary>
    /// <param name="property">The property to compute the next revision for.</param>
    /// <returns>The next revision slot.</returns>
    IPropertyStorageRevisionSlot GetNextPropertyStorageRevisionSlot(IProperty property);

    /// <summary>
    /// Gets the storage revision slot for a specific version of the specified property.
    /// </summary>
    /// <param name="property">The property descriptor to look up.</param>
    /// <param name="version">The version number to retrieve.</param>
    /// <returns>The revision slot for the specified version.</returns>
    IPropertyStorageRevisionSlot GetPropertyStorageRevisionSlot(IPropertyDescriptor property, int version);

    /// <summary>
    /// Gets the latest revision number for the specified property, or 0 if no revisions exist.
    /// </summary>
    /// <param name="property">The property descriptor to look up.</param>
    /// <returns>The latest revision number, or 0 if none exist.</returns>
    int GetLatestRevisionNumber(IPropertyDescriptor property);

    /// <summary>
    /// Gets the next revision number for the specified property (latest + 1).
    /// </summary>
    /// <param name="property">The property to compute the next revision number for.</param>
    /// <returns>The next revision number.</returns>
    int GetNextRevisionNumber(IProperty property);

    /// <summary>
    /// Determines whether the specified property's current value is equal to its latest stored revision.
    /// </summary>
    /// <param name="property">The property to compare.</param>
    /// <returns>True if the current value matches the latest revision; otherwise, false.</returns>
    bool IsEqualToLatestRevision(IProperty property);

    /// <summary>
    /// Determines whether a revision exists for the specified property at the given revision number.
    /// </summary>
    /// <param name="property">The property descriptor to check.</param>
    /// <param name="revisionNumber">The revision number to check for. Defaults to 1.</param>
    /// <returns>True if the revision exists; otherwise, false.</returns>
    bool RevisionExists(IPropertyDescriptor property, int revisionNumber = 1);

    /// <summary>
    /// Reads all revisions for the specified property.
    /// </summary>
    /// <param name="parent">The parent object data.</param>
    /// <param name="propertyDescriptor">The property descriptor to read revisions for.</param>
    /// <returns>An enumerable of property revisions.</returns>
    IEnumerable<IPropertyRevision> ReadRevisions(IObjectData parent, IPropertyDescriptor propertyDescriptor);

    /// <summary>
    /// Gets all version storage slots for the specified property.
    /// </summary>
    /// <param name="property">The property descriptor to get version slots for.</param>
    /// <returns>An enumerable of property storage revision slots.</returns>
    IEnumerable<IPropertyStorageRevisionSlot> GetPropertyStorageVersionSlots(IPropertyDescriptor property);

    /// <summary>
    /// Writes the specified property to the next revision slot in storage.
    /// </summary>
    /// <param name="propertyDescriptor">The property to write.</param>
    /// <returns>The result of the write operation.</returns>
    IPropertyWriteResult WriteProperty(IProperty propertyDescriptor);

    /// <summary>
    /// Reads the latest revision of the specified property from storage.
    /// </summary>
    /// <param name="parent">The parent object data.</param>
    /// <param name="propertyDescriptor">The property descriptor to read.</param>
    /// <returns>The property, or null if not found.</returns>
    IProperty? ReadProperty(IObjectData parent, IPropertyDescriptor propertyDescriptor);

    /// <summary>
    /// Writes all properties of the specified object data to storage.
    /// </summary>
    /// <param name="data">The object data to write.</param>
    /// <returns>The result of the write operation, including per-property results.</returns>
    IObjectDataWriteResult WriteObject(IObjectData data);

    /// <summary>
    /// Reads an object from storage by its key, reconstructing all stored properties.
    /// </summary>
    /// <param name="objectDataKey">The key identifying the object to read.</param>
    /// <returns>The reconstructed object data.</returns>
    IObjectData ReadObject(IObjectDataKey objectDataKey);
}
