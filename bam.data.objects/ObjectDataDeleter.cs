namespace Bam.Data.Objects;

public class ObjectDataDeleter : IObjectDataDeleter
{
    public Task<IObjectDataDeleteResult> DeleteAsync(object data)
    {
        throw new NotImplementedException();
    }

    public Task<IObjectDataDeleteResult> DeleteAsync(IObjectData data)
    {
        throw new NotImplementedException();
    }
}