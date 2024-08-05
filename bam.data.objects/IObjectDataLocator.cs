using Bam.Data.Objects;
using Bam.Storage;

namespace Bam.Data.Objects;

public interface IObjectDataLocator
{
    IStorageIdentifier StorageIdentifier { get; }
    IObjectDataKey ObjectDataKey { get; }
    IObjectDataIdentifier ObjectDataIdentifier { get; }
}