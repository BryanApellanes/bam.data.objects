using Bam.Data.Objects;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

public class ObjectDataIdentifierFactory : IObjectDataIdentifierFactory
{
    public ObjectDataIdentifierFactory(IObjectDataIdentityCalculator objectDataIdentityCalculator)
    {
        this.ObjectDataIdentityCalculator = objectDataIdentityCalculator;
    }
    private IObjectDataIdentityCalculator ObjectDataIdentityCalculator { get; init; }

    public IObjectDataKey GetObjectKey(IObjectData data)
    {
        return new ObjectDataKey()
        {
            TypeDescriptor = data.Type,
            //StorageIdentifier = objectDataStorageManager.GetObjectStorageHolder(data.Type),
            Key = ObjectDataIdentityCalculator.CalculateHashHexKey(data),
            Id = ObjectDataIdentityCalculator.CalculateHashHex(data)
        };
    }

    public IObjectDataIdentifier GetObjectIdentifier(IObjectData data)
    {
        return new ObjectDataIdentifier()
        {
            TypeDescriptor = data.Type,
            //StorageIdentifier = objectDataStorageManager.GetObjectStorageHolder(data.Type),
            Id = ObjectDataIdentityCalculator.CalculateHashHex(data)
        };
    }
}