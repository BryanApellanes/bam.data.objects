namespace Bam.Data.Objects;

public interface IObjectDataIndexer
{
    Task<IObjectDataIndexResult> IndexAsync(object data);
    Task<IObjectDataIndexResult> IndexAsync(IObjectData data);
    Task<IObjectDataKey?> LookupAsync<T>(ulong id);
    Task<IObjectDataKey?> LookupAsync(Type type, ulong id);
    Task<IObjectDataKey?> LookupByUuidAsync<T>(string uuid);
    Task<IObjectDataKey?> LookupByUuidAsync(Type type, string uuid);
    Task<IEnumerable<IObjectDataKey>> GetAllKeysAsync(Type type);
    Task<IEnumerable<IObjectDataKey>> GetAllKeysAsync<T>();
}
