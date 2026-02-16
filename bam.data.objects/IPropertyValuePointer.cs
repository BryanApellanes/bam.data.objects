using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Represents a pointer-value pair in storage, where the pointer slot references the value slot via a hash.
/// </summary>
public interface IPropertyValuePointer
{
    /// <summary>
    /// Gets or sets the storage slot containing the hash pointer to the raw data.
    /// </summary>
    IStorageSlot PointerStorageSlot { get; set; }

    /// <summary>
    /// Gets or sets the storage slot containing the actual raw data value.
    /// </summary>
    IStorageSlot ValueStorageSlot { get; set; }

}