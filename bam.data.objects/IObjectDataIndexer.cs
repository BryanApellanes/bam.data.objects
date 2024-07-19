namespace Bam.Data.Objects;

public interface IObjectDataIndexer
{
    Task<IObjectDataIndexResult> IndexAsync(object data);
    Task<IObjectDataIndexResult> IndexAsync(IObjectData data);
}