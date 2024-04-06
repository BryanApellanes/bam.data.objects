using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectKey : IObjectKey
{
    public TypeDescriptor Type { get; set; }
    public IStorageIdentifier StorageIdentifier { get; internal set; }
    public ulong Hash { get; internal set; }
    public ulong Key { get; internal set; }
}