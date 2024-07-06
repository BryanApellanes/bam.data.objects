using Bam.Storage;

namespace Bam.Data.Objects;

public interface ILookupStorageHolder : IStorageHolder
{
    Type Type { get; }
    IRootStorageHolder RootStorageHolder { get; }
}