using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectIdentifier : IObjectIdentifier
{
    public TypeDescriptor Type { get; set; }
    public IStorageIdentifier StorageIdentifier { get; set; }
    public ulong Id { get; set; }
}