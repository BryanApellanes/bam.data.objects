namespace Bam.Data.Objects;

public interface IObjectDataArchiver
{
    Task<IObjectDataArchiveResult> ArchiveAsync(object data);
    Task<IObjectDataArchiveResult> ArchiveAsync(IObjectData data);
}