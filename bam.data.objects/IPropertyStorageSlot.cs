using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IPropertyStorageSlot : IStorageSlot
{
    IPropertyHolder PropertyHolder { get; }
    
    IPropertyVersion PropertyVersion { get; }
    IList<IPropertyVersion> VersionHistory { get; }
    
    IPropertyWriteResult Save(IObjectStorageManager storageManager, IProperty property);
}