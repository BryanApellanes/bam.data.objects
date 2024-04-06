using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectDataWriteResult : IResult
{
    IObjectData Data { get; }
}