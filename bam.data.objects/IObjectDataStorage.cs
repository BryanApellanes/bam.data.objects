namespace Bam.Storage;

public interface IObjectDataStorage : IStorage
{
    IObjectDataStorageSaveResult SaveObject(object data);
    IObjectDataStorageSaveResult<T> SaveObject<T>(T data);
    IObjectDataStorageLoadResult LoadObject(string hash);
    IObjectStorageLoadResult<T> LoadObject<T>(string hash);
}