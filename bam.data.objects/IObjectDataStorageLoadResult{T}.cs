namespace Bam.Storage;

public interface IObjectStorageLoadResult<T>
{
    new IObjectData<T> Data { get; }
}