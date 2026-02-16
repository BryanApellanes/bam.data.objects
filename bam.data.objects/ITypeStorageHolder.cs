using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Represents a storage holder scoped to a specific type, providing the root storage context.
/// </summary>
public interface ITypeStorageHolder : IStorageHolder
{
    /// <summary>
    /// Gets the root storage holder that contains this type-scoped storage.
    /// </summary>
    IRootStorageHolder RootStorageHolder { get; }
}