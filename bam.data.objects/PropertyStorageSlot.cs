using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public abstract class PropertyStorageSlot : FsStorageSlot, IPropertyStorageSlot
{
    public PropertyStorageSlot(PropertyStorageHolder storageHolder)
    {
        this.StorageHolder = storageHolder;
    }
    
    public IPropertyHolder PropertyHolder { get; }
    
    public IPropertyVersion PropertyVersion { get; }
    
    public IList<IPropertyVersion> VersionHistory { get; }
    public abstract IPropertyWriteResult Save(IObjectStorageManager storageManager, IProperty property);
}