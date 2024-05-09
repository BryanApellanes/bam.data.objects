using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

public interface IObjectPropertyWriteResult
{
    IStorageSlot StorageSlot { get; set; }
    IObjectProperty ObjectProperty { get; set; }
    IRawData RawData { get; set; }
    bool Success { get; set; }
    string Message { get; set; }
    string RawDataHash { get; set; }
}