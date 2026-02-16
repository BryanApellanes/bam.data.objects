using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Represents a storage holder that supports lookup operations, associating a type with a root storage location.
/// </summary>
public interface ILookupStorageHolder : IStorageHolder
{
    /// <summary>
    /// Gets the type associated with this storage holder.
    /// </summary>
    Type Type { get; }

    /// <summary>
    /// Gets the root storage holder that contains this lookup storage.
    /// </summary>
    IRootStorageHolder RootStorageHolder { get; }
}