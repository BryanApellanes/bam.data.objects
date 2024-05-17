using Bam.Data.Objects;

namespace bam.data.objects;

public interface IObjectDeleter
{
    Task<IObjectDataDeleteResult> DeleteAsync(object data);
    Task<IObjectDataDeleteResult> DeleteAsync(IObjectData data);
}