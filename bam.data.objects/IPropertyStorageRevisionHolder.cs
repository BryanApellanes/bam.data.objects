namespace Bam.Data.Objects;

/// <summary>
/// Represents a property storage holder scoped to a specific revision version.
/// </summary>
public interface IPropertyStorageRevisionHolder: IPropertyStorageHolder
{
    /// <summary>
    /// Gets the version number of this revision.
    /// </summary>
    int Version { get; }
}