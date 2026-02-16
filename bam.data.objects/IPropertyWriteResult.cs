using Bam.Data.Objects;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

/// <summary>
/// Represents the result of writing a property to storage, including pointer and value slot information.
/// </summary>
public interface IPropertyWriteResult
{
    /// <summary>
    /// Gets or sets the object key of the parent object the property belongs to.
    /// </summary>
    IObjectDataKey ObjectDataKey { get; set; }

    /// <summary>
    /// Gets or sets the storage slot containing the hash pointer to the raw data.
    /// </summary>
    IStorageSlot PointerStorageSlot { get; set; }

    /// <summary>
    /// Gets or sets the storage slot containing the actual raw data value.
    /// </summary>
    IStorageSlot ValueStorageSlot { get; set; }

    /// <summary>
    /// Gets or sets the property that was written.
    /// </summary>
    IProperty Property { get; set; }

    /// <summary>
    /// Gets or sets the raw data that was persisted.
    /// </summary>
    IRawData RawData { get; set; }

    /// <summary>
    /// Gets or sets the status of the property write operation.
    /// </summary>
    PropertyWriteResults Status { get; set; }

    /// <summary>
    /// Gets or sets a message describing the result, typically populated on failure.
    /// </summary>
    string Message { get; set; }

    /// <summary>
    /// Gets or sets the hash hex string of the raw data.
    /// </summary>
    string RawDataHash { get; set; }

    /// <summary>
    /// Creates a property descriptor from this write result.
    /// </summary>
    /// <returns>A property descriptor containing the object key and property name.</returns>
    IPropertyDescriptor GetDescriptor();
}