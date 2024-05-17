namespace Bam.Data.Objects;

public interface IObjectIndexer
{
    Task<IObjectDataIndexResult> IndexAsync(object data);
    Task<IObjectDataIndexResult> IndexAsync(IObjectData data);
}