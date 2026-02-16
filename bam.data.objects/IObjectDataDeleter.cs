namespace Bam.Data.Objects;

/// <summary>
/// Defines operations for deleting object data from storage, including associated index entries.
/// </summary>
public interface IObjectDataDeleter
{
    /// <summary>
    /// Deletes the specified data object and its associated storage and index entries asynchronously.
    /// </summary>
    /// <param name="data">The object to delete.</param>
    /// <returns>The result of the delete operation.</returns>
    Task<IObjectDataDeleteResult> DeleteAsync(object data);

    /// <summary>
    /// Deletes the specified object data wrapper and its associated storage and index entries asynchronously.
    /// </summary>
    /// <param name="data">The object data to delete.</param>
    /// <returns>The result of the delete operation.</returns>
    Task<IObjectDataDeleteResult> DeleteAsync(IObjectData data);
}