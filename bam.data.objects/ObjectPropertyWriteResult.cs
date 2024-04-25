using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;



public class ObjectPropertyWriteResult : IObjectPropertyWriteResult
{
    public IStorageSlot StorageIdentifier { get; set; }
    public IObjectProperty ObjectProperty { get; set; }
    public IRawData RawData { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public string RawDataHash { get; set; }
}