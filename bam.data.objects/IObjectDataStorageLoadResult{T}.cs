namespace Bam.Storage;

/// <summary>
/// Represents the strongly-typed result of loading object data from storage.
/// </summary>
/// <typeparam name="T">The type of the loaded object.</typeparam>
public interface IObjectStorageLoadResult<T>
{
    /// <summary>
    /// Gets the loaded strongly-typed object data, or null if the load failed.
    /// </summary>
    IObjectData<T> Data { get; }
}