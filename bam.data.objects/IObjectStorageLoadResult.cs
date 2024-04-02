using Bam.Data.Objects;

namespace Bam.Storage;

public interface IObjectStorageLoadResult
{
    IObjectData Data { get; }
    bool Success { get; }
    string Message { get; }
}