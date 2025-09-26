using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public abstract class PropertyStorageSlot : FsStorageSlot, IPropertyStorageSlot
{
    public PropertyStorageSlot(IPropertyStorageHolder storageHolder)
    {
        this.StorageHolder = storageHolder;
    }
    
    public IPropertyStorageHolder PropertyStorageHolder { get; }
    
    public IPropertyRevision PropertyRevision { get; }
    
    public IList<IPropertyRevision> VersionHistory { get; }
    public abstract IPropertyWriteResult Save(IObjectDataStorageManager dataStorageManager, IProperty property);
}