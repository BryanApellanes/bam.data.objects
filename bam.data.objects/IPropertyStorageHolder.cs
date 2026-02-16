using Bam.Data.Dynamic.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Represents a storage holder for a specific property, providing versioned save and retrieval operations.
/// </summary>
public interface IPropertyStorageHolder : IStorageHolder
{
    /// <summary>
    /// Gets the name of the property this holder manages.
    /// </summary>
    string PropertyName { get; }

    /// <summary>
    /// Gets the type storage holder that this property storage resides under.
    /// </summary>
    ITypeStorageHolder TypeStorageHolder { get; }

    /// <summary>
    /// Gets the revision slot for a specific version of this property.
    /// </summary>
    /// <param name="objectDataStorageManager">The storage manager to use for resolution.</param>
    /// <param name="property">The property to get the version slot for.</param>
    /// <param name="version">The version number to retrieve.</param>
    /// <returns>The property storage revision slot for the specified version.</returns>
    IPropertyStorageRevisionSlot GetPropertyVersionSlot(IObjectDataStorageManager objectDataStorageManager, IProperty property, int version);

    /// <summary>
    /// Saves the property to storage, creating a new revision if the value has changed, or returning the existing revision if unchanged.
    /// </summary>
    /// <param name="dataStorageManager">The storage manager to use.</param>
    /// <param name="property">The property to save.</param>
    /// <returns>The result of the save operation.</returns>
    IPropertyWriteResult Save(IObjectDataStorageManager dataStorageManager, IProperty property);

    /// <summary>
    /// Gets all version slots for the specified property.
    /// </summary>
    /// <param name="dataStorageManager">The storage manager to use.</param>
    /// <param name="property">The property to get versions for.</param>
    /// <returns>An enumerable of revision slots.</returns>
    IEnumerable<IPropertyStorageRevisionSlot> GetVersions(IObjectDataStorageManager dataStorageManager, IProperty property);
}