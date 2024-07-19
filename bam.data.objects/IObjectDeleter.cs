using Bam.Data.Objects;

namespace Bam.Data.Objects;

public interface IObjectDeleter
{
    Task<IObjectDataDeleteResult> DeleteAsync(object data);
    Task<IObjectDataDeleteResult> DeleteAsync(IObjectData data);
}