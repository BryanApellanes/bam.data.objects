namespace Bam.Storage;

public interface IFsObjectStorage
{
    IObjectStorageSaveResult SaveObject(string rootPath, object data);
    IObjectStorageSaveResult<T> SaveObject<T>(string rootPath, T data);
    IObjectStorageLoadResult LoadObject(string rootPath, string hash);
    IObjectStorageLoadResult<T> LoadObject<T>(string rootPath, string hash);
}