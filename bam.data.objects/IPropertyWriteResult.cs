using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

public interface IPropertyWriteResult
{
    IStorageSlot StorageSlot { get; set; }
    IProperty Property { get; set; }
    IRawData RawData { get; set; }
    bool Success { get; set; }
    string Message { get; set; }
    string RawDataHash { get; set; }
}