namespace Bam.Data.Objects;

/// <summary>
/// Defines operations for archiving object data.
/// </summary>
public interface IObjectDataArchiver
{
    /// <summary>
    /// Archives the specified data object asynchronously.
    /// </summary>
    /// <param name="data">The object to archive.</param>
    /// <returns>The result of the archive operation.</returns>
    Task<IObjectDataArchiveResult> ArchiveAsync(object data);

    /// <summary>
    /// Archives the specified object data wrapper asynchronously.
    /// </summary>
    /// <param name="data">The object data to archive.</param>
    /// <returns>The result of the archive operation.</returns>
    Task<IObjectDataArchiveResult> ArchiveAsync(IObjectData data);
}