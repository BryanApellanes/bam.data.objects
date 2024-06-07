using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IPropertyStorageSlot : IStorageSlot
{
    IPropertyStorageHolder PropertyStorageHolder { get; }
    
    IPropertyVersion PropertyVersion { get; }
    IList<IPropertyVersion> VersionHistory { get; }
    
    IPropertyWriteResult Save(IObjectStorageManager storageManager, IProperty property);
}