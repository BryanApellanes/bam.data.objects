namespace Bam.Data.Objects;

/// <summary>
/// Defines the operation for reading object data from storage by its key.
/// </summary>
public interface IObjectDataReader
{
    /// <summary>
    /// Reads object data from storage using the specified key asynchronously.
    /// </summary>
    /// <param name="dataKey">The key identifying the object to read.</param>
    /// <returns>The result of the read operation, containing the object data if successful.</returns>
    Task<IObjectDataReadResult> ReadObjectDataAsync(IObjectDataKey dataKey);
}