using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectDataObjectStorage : IObjectStorage
{
    IObjectDataStorageSaveResult SaveObject(object data);
    IObjectDataStorageSaveResult<T> SaveObject<T>(T data);
    IObjectDataStorageLoadResult LoadObject(string hash);
    IObjectStorageLoadResult<T> LoadObject<T>(string hash);
}