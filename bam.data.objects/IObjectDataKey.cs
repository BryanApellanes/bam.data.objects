namespace Bam.Data.Objects;

/// <summary>
/// Represents a composite-key-based identifier for an object, derived from properties adorned with <see cref="Bam.Data.Repositories.CompositeKeyAttribute"/>.
/// </summary>
public interface IObjectDataKey
{
    /// <summary>
    /// Gets the type descriptor for the identified object.
    /// </summary>
    TypeDescriptor TypeDescriptor { get; }

    /// <summary>
    /// Gets the hex-encoded composite key hash for the object.
    /// </summary>
    string? Key { get; }

    /// <summary>
    /// Constructs the file system path for this key, optionally prefixed with the storage root path.
    /// </summary>
    /// <param name="objectDataStorageManager">The storage manager used to resolve the root path, or null to omit the root prefix.</param>
    /// <returns>The resolved storage path for this key.</returns>
    string GetPath(IObjectDataStorageManager? objectDataStorageManager);

    /// <summary>
    /// Creates a property descriptor for the specified property name, associated with this object key.
    /// </summary>
    /// <param name="propertyName">The name of the property.</param>
    /// <returns>A property descriptor bound to this key.</returns>
    IPropertyDescriptor Property(string propertyName);
}