namespace Bam.Data.Objects;

public interface IObjectLoader
{
    Task<IObjectDataLoadResult> LoadAsync(IObjectKey key);
}