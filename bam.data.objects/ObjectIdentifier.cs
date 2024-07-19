using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectIdentifier : IObjectIdentifier
{
    public TypeDescriptor TypeDescriptor { get; set; }
    public IStorageIdentifier StorageIdentifier { get; set; }
    public string Id { get; set; }
}