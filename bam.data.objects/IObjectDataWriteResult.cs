using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectDataWriteResult : IResult
{
    IObjectData ObjectData { get; }
    IObjectDataKey ObjectDataKey { get; }
    IStorageSlot KeySlot { get; }
    IDictionary<string, IPropertyWriteResult> PropertyWriteResults { get; } 
}