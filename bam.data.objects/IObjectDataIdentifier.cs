using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectDataIdentifier
{
    TypeDescriptor TypeDescriptor { get; }
    IStorageIdentifier StorageIdentifier { get; }
    string Id { get; }
}