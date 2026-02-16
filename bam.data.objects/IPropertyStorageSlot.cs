using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Represents a storage slot for a property, supporting save operations and version history tracking.
/// </summary>
public interface IPropertyStorageSlot : IStorageSlot
{
    /// <summary>
    /// Gets the property storage holder that contains this slot.
    /// </summary>
    IPropertyStorageHolder PropertyStorageHolder { get; }

    /// <summary>
    /// Gets the property revision associated with this slot.
    /// </summary>
    IPropertyRevision PropertyRevision { get; }

    /// <summary>
    /// Gets the version history of the property.
    /// </summary>
    IList<IPropertyRevision> VersionHistory { get; }

    /// <summary>
    /// Saves the specified property to this storage slot.
    /// </summary>
    /// <param name="dataStorageManager">The storage manager to use for the write operation.</param>
    /// <param name="property">The property to save.</param>
    /// <returns>The result of the save operation.</returns>
    IPropertyWriteResult Save(IObjectDataStorageManager dataStorageManager, IProperty property);
}