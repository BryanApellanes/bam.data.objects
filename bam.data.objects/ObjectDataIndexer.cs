namespace Bam.Data.Objects;

public class ObjectDataIndexer : IObjectDataIndexer
{
    public Task<IObjectDataIndexResult> IndexAsync(object data)
    {
        throw new NotImplementedException();
    }

    public Task<IObjectDataIndexResult> IndexAsync(IObjectData data)
    {
        throw new NotImplementedException();
    }
}