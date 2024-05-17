using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectKey : IObjectKey
{
    public TypeDescriptor Type { get; set; }
    public IStorageIdentifier StorageIdentifier { get; internal set; }
    public string Id { get; internal set; }
    public string Key { get; internal set; }
}