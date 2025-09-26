using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IPropertyStorageSlot : IStorageSlot
{
    IPropertyStorageHolder PropertyStorageHolder { get; }
    
    IPropertyRevision PropertyRevision { get; }
    IList<IPropertyRevision> VersionHistory { get; }
    
    IPropertyWriteResult Save(IObjectDataStorageManager dataStorageManager, IProperty property);
}