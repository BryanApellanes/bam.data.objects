namespace Bam.Data.Objects;

public interface IObjectDataSearchIndexer
{
    Task<IObjectDataSearchIndexResult> IndexAsync(IObjectData data);
    Task RemoveAsync(IObjectData data);
    Task<IEnumerable<IObjectDataKey>> LookupAsync(Type type, string propertyName, string valueHash);
    Task RebuildAsync(Type type);
    Task RebuildAsync<T>();
}
