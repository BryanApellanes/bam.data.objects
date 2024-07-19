using Bam.Storage;

namespace Bam.Data.Objects;

public class TypeStorageHolder : DirectoryStorageHolder, ITypeStorageHolder
{
    public TypeStorageHolder(string path) : base(path)
    {
    }

    public IRootStorageHolder RootStorageHolder { get; internal set; }
}