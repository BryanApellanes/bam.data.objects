namespace Bam.Storage;

public interface IObjectStorage
{
    IObjectStorageSaveResult SaveObject(object data);
    IObjectStorageSaveResult<T> SaveObject<T>(T data);
    IObjectStorageLoadResult LoadObject(string hash);
    IObjectStorageLoadResult<T> LoadObject<T>(string hash);
}