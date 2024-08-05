using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectDataLocator : IObjectDataLocator
{
    public IStorageIdentifier StorageIdentifier { get; init; }
    public IObjectDataKey ObjectDataKey { get; set;}
    public IObjectDataIdentifier ObjectDataIdentifier { get; set; }
}