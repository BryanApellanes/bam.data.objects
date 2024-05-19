using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;



public class PropertyWriteResult : IPropertyWriteResult
{
    public IStorageSlot StorageSlot { get; set; }
    public IProperty Property { get; set; }
    public IRawData RawData { get; set; }
    public bool Success { get; set; }
    public string Message { get; set; }
    public string RawDataHash { get; set; }
}