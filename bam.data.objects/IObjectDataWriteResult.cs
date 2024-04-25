using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectDataWriteResult : IResult
{
    IObjectData Data { get; }
    IObjectKey ObjectKey { get; }
    IDictionary<string, IObjectPropertyWriteResult> PropertyWriteResults { get; } 
}