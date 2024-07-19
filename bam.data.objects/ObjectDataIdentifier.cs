using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectDataIdentifier : IObjectDataIdentifier
{
    public TypeDescriptor TypeDescriptor { get; set; }
    public IStorageIdentifier StorageIdentifier { get; set; }
    public string Id { get; set; }
}