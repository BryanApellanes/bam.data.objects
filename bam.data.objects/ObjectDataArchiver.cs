namespace Bam.Data.Objects;

public class ObjectDataArchiver: IObjectDataArchiver
{
    public Task<IObjectDataArchiveResult> ArchiveAsync(object data)
    {
        throw new NotImplementedException();
    }

    public Task<IObjectDataArchiveResult> ArchiveAsync(IObjectData data)
    {
        throw new NotImplementedException();
    }
}