using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectIdentifier
{
    TypeDescriptor Type { get; }
    IStorageIdentifier StorageIdentifier { get; }
    string Id { get; }
}