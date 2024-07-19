namespace Bam.Storage;

public interface IFsObjectStorage
{
    IObjectDataStorageSaveResult SaveObject(string rootPath, object data);
    IObjectDataStorageSaveResult<T> SaveObject<T>(string rootPath, T data);
    IObjectDataStorageLoadResult LoadObject(string rootPath, string hash);
    IObjectStorageLoadResult<T> LoadObject<T>(string rootPath, string hash);
}