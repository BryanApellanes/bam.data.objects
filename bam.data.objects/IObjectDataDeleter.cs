namespace Bam.Data.Objects;

public interface IObjectDataDeleter
{
    Task<IObjectDataDeleteResult> DeleteAsync(object data);
    Task<IObjectDataDeleteResult> DeleteAsync(IObjectData data);
}