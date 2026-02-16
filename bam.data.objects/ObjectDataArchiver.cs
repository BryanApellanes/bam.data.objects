namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataArchiver"/>. Not yet implemented.
/// </summary>
public class ObjectDataArchiver: IObjectDataArchiver
{
    /// <inheritdoc />
    public Task<IObjectDataArchiveResult> ArchiveAsync(object data)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public Task<IObjectDataArchiveResult> ArchiveAsync(IObjectData data)
    {
        throw new NotImplementedException();
    }
}