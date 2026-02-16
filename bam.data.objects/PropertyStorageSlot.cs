using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Abstract base implementation of <see cref="IPropertyStorageSlot"/> that provides file-system-based property storage.
/// </summary>
public abstract class PropertyStorageSlot : FsStorageSlot, IPropertyStorageSlot
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyStorageSlot"/> class.
    /// </summary>
    /// <param name="storageHolder">The property storage holder that contains this slot.</param>
    public PropertyStorageSlot(IPropertyStorageHolder storageHolder)
    {
        this.StorageHolder = storageHolder;
    }
    
    /// <inheritdoc />
    public IPropertyStorageHolder PropertyStorageHolder { get; }

    /// <inheritdoc />
    public IPropertyRevision PropertyRevision { get; }

    /// <inheritdoc />
    public IList<IPropertyRevision> VersionHistory { get; }

    /// <inheritdoc />
    public abstract IPropertyWriteResult Save(IObjectDataStorageManager dataStorageManager, IProperty property);
}