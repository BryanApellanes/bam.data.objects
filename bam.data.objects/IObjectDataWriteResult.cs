using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Represents the result of writing object data to storage, including per-property write results.
/// </summary>
public interface IObjectDataWriteResult : IResult
{
    /// <summary>
    /// Gets the object data that was written.
    /// </summary>
    IObjectData ObjectData { get; }

    /// <summary>
    /// Gets the key of the written object.
    /// </summary>
    IObjectDataKey ObjectDataKey { get; }

    /// <summary>
    /// Gets the storage slot where the object key is stored.
    /// </summary>
    IStorageSlot KeySlot { get; }

    /// <summary>
    /// Gets the dictionary of property write results keyed by property name.
    /// </summary>
    IDictionary<string, IPropertyWriteResult> PropertyWriteResults { get; }
}