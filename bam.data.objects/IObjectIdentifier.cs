using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectIdentifier
{
    TypeDescriptor TypeDescriptor { get; }
    IStorageIdentifier StorageIdentifier { get; }
    string Id { get; }
}