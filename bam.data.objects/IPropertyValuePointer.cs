using Bam.Storage;

namespace Bam.Data.Objects;

public interface IPropertyValuePointer
{
    IStorageSlot PointerStorageSlot { get; set; }
    IStorageSlot ValueStorageSlot { get; set; }
    
}