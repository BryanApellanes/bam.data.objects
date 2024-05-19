using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectDataWriteResult : IResult
{
    IObjectData ObjectData { get; }
    IObjectKey ObjectKey { get; }
    IStorageSlot KeySlot { get; }
    IDictionary<string, IPropertyWriteResult> PropertyWriteResults { get; } 
}