namespace Bam.Data.Objects;

/// <summary>
/// Defines operations for writing object data to storage.
/// </summary>
public interface IObjectDataWriter
{
    /// <summary>
    /// Writes the specified data object to storage asynchronously, wrapping it in an <see cref="IObjectData"/> if needed.
    /// </summary>
    /// <param name="data">The object to write.</param>
    /// <returns>The result of the write operation.</returns>
    Task<IObjectDataWriteResult> WriteAsync(object data);

    /// <summary>
    /// Writes the specified object data to storage asynchronously.
    /// </summary>
    /// <param name="data">The object data to write.</param>
    /// <returns>The result of the write operation.</returns>
    Task<IObjectDataWriteResult> WriteAsync(IObjectData data);
}