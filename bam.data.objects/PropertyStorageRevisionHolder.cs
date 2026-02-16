namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="IPropertyStorageRevisionHolder"/> that extends property storage with a specific revision version subdirectory.
/// </summary>
public class PropertyStorageRevisionHolder : PropertyStorageHolder, IPropertyStorageRevisionHolder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyStorageRevisionHolder"/> class with the specified path and version.
    /// </summary>
    /// <param name="path">The file system path for the property storage directory.</param>
    /// <param name="version">The revision version number.</param>
    public PropertyStorageRevisionHolder(string path, int version) : base(path)
    {
        this.Version = version;
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PropertyStorageRevisionHolder"/> class with the specified directory and version.
    /// </summary>
    /// <param name="directory">The directory info for the property storage directory.</param>
    /// <param name="version">The revision version number.</param>
    public PropertyStorageRevisionHolder(DirectoryInfo directory, int version) : base(directory)
    {
        this.Version = version;
    }

    /// <inheritdoc />
    public int Version { get; }

    /// <summary>
    /// Gets the full path including the version subdirectory.
    /// </summary>
    public override string? FullName => Path.Combine(base.FullName, Version.ToString());
}