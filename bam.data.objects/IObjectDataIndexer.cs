namespace Bam.Data.Objects;

public interface IObjectDataIndexer
{
    Task<IObjectDataIndexResult> IndexAsync(object data);
    Task<IObjectDataIndexResult> IndexAsync(IObjectData data);
    Task<IObjectDataKey?> LookupAsync<T>(ulong id);
    Task<IObjectDataKey?> LookupAsync(Type type, ulong id);
}
