namespace Bam.Data.Objects;

public interface IObjectArchiver
{
    Task<IObjectDataArchiveResult> ArchiveAsync(object data);
    Task<IObjectDataArchiveResult> ArchiveAsync(IObjectData data);
}