using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectDataIdentifier
{
    TypeDescriptor TypeDescriptor { get; }
    IStorageIdentifier GetStorageIdentifier(IObjectDataStorageManager objectDataStorageManager);
    string? Id { get; }
}