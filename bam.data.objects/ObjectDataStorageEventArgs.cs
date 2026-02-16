using Bam.Data.Dynamic.Objects;
using Bam.Data.Objects;
using Bam.Storage;

namespace Bamn.Data.Objects;

/// <summary>
/// Provides event data for object data storage operations, such as property reads and writes.
/// </summary>
public class ObjectDataStorageEventArgs : EventArgs
{
    /// <summary>
    /// Gets or sets the result of the property write operation, if applicable.
    /// </summary>
    public IPropertyWriteResult PropertyWriteResult { get; set; }

    /// <summary>
    /// Gets or sets the exception that occurred during the storage operation, if any.
    /// </summary>
    public Exception Exception { get; set; }

    /// <summary>
    /// Gets or sets the property descriptor for the property being read or written.
    /// </summary>
    public IPropertyDescriptor PropertyDescriptor { get; set; }

    /// <summary>
    /// Gets or sets the storage slot being read from, if applicable.
    /// </summary>
    public IStorageSlot ReadingFrom { get; set; }
}