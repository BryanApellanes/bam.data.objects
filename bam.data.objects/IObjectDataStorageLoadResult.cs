using Bam.Data.Objects;

namespace Bam.Storage;

public interface IObjectDataStorageLoadResult
{
    IObjectData Data { get; }
    bool Success { get; }
    string Message { get; }
}