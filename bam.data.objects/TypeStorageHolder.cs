using Bam.Storage;

namespace Bam.Data.Objects;

/// <summary>
/// Default implementation of <see cref="ITypeStorageHolder"/> that represents a directory-based storage holder scoped to a specific type.
/// </summary>
public class TypeStorageHolder : DirectoryStorageHolder, ITypeStorageHolder
{
    /// <summary>
    /// Initializes a new instance of the <see cref="TypeStorageHolder"/> class with the specified directory path.
    /// </summary>
    /// <param name="path">The file system path for the type storage directory.</param>
    public TypeStorageHolder(string path) : base(path)
    {
    }

    /// <inheritdoc />
    public IRootStorageHolder RootStorageHolder { get; internal set; }
}