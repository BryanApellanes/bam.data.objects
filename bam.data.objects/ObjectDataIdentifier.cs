using Bam.Storage;

namespace Bam.Data.Objects;

public class ObjectDataIdentifier : IObjectDataIdentifier
{
    public TypeDescriptor TypeDescriptor { get; set; }
    
    public IStorageIdentifier GetStorageIdentifier(IObjectDataStorageManager objectDataStorageManager)
    {
        return objectDataStorageManager.GetObjectStorageHolder(this.TypeDescriptor);
    }

    public string Id { get; set; }
}