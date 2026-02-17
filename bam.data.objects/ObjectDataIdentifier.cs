using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataIdentifier"/>, providing a content-hash-based identifier for an object.
/// </summary>
public class ObjectDataIdentifier : IObjectDataIdentifier
{
    /// <inheritdoc />
    public TypeDescriptor TypeDescriptor { get; set; } = null!;

    /// <inheritdoc />
    public IStorageIdentifier GetStorageIdentifier(IObjectDataStorageManager objectDataStorageManager)
    {
        return objectDataStorageManager.GetObjectStorageHolder(this.TypeDescriptor);
    }

    /// <inheritdoc />
    public string? Id { get; set; }
}