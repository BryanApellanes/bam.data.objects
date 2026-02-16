namespace Bam.Data.Objects;

/// <summary>
/// Represents a storage slot for a specific revision of a property, including metadata.
/// </summary>
public interface IPropertyStorageRevisionSlot : IPropertyStorageSlot
{
    /// <summary>
    /// Gets the revision holder that contains this slot.
    /// </summary>
    IPropertyStorageRevisionHolder PropertyStorageRevisionHolder { get; }

    /// <summary>
    /// Gets the revision number of this slot.
    /// </summary>
    int Revision { get; }

    /// <summary>
    /// Gets or sets the metadata associated with this revision.
    /// </summary>
    string MetaData { get; set; }
}