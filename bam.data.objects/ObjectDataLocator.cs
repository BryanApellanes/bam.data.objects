using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataLocator"/>, combining storage location, composite key, and content identifier.
/// </summary>
public class ObjectDataLocator : IObjectDataLocator
{
    /// <inheritdoc />
    public IStorageIdentifier StorageIdentifier { get; init; }

    /// <inheritdoc />
    public IObjectDataKey ObjectDataKey { get; set;}

    /// <inheritdoc />
    public IObjectDataIdentifier ObjectDataIdentifier { get; set; }
}