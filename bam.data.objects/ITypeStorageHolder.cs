using Bam.Storage;

namespace Bam.Data.Objects;

public interface ITypeStorageHolder : IStorageHolder
{
    IRootStorageHolder RootStorageHolder { get; }
}