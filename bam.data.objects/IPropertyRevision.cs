namespace Bam.Data.Objects;

/// <summary>
/// Represents a specific revision of a property value, including its version number and metadata.
/// </summary>
public interface IPropertyRevision
{
    /// <summary>
    /// Gets the type descriptor of the parent object, or null if not available.
    /// </summary>
    ITypeDescriptor? TypeDescriptor { get; }

    /// <summary>
    /// Gets the parent object data that owns the property, or null if not available.
    /// </summary>
    IObjectData? Parent { get; }

    /// <summary>
    /// Gets the property associated with this revision.
    /// </summary>
    IProperty Property { get; }

    /// <summary>
    /// Gets the revision number (1-based).
    /// </summary>
    int Number { get; }

    /// <summary>
    /// Gets the metadata associated with this revision.
    /// </summary>
    string MetaData { get; }
}