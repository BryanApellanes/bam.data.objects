using Bam.Data.Dynamic.Objects;

namespace Bam.Data.Objects;

public interface IPropertyStorageVersionSlot : IPropertyStorageSlot
{
    IPropertyStorageVersionHolder PropertyStorageVersionHolder { get; }
    int Version { get; }

    IProperty Load(IObjectStorageManager storageManager);
}