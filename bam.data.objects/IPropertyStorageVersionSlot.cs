using Bam.Data.Dynamic.Objects;

namespace Bam.Data.Objects;

public interface IPropertyStorageVersionSlot : IPropertyStorageSlot
{
    int Version { get; }

    IProperty Load(IObjectStorageManager storageManager);
}