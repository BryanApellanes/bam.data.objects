using Bam.Data.Objects;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

public class ObjectIdentifierFactory : IObjectIdentifierFactory
{
    public ObjectIdentifierFactory(IObjectStorageManager objectStorageManager, IObjectIdentityCalculator objectIdentityCalculator)
    {
        this.ObjectStorageManager = objectStorageManager;
        this.ObjectIdentityCalculator = objectIdentityCalculator;
    }

    private IObjectStorageManager ObjectStorageManager { get; init; }
    private IObjectIdentityCalculator ObjectIdentityCalculator { get; init; }

    public IObjectKey GetObjectKey(IObjectData data)
    {
        return new ObjectKey()
        {
            Type = data.Type,
            StorageIdentifier = ObjectStorageManager.GetObjectStorageHolder(data.Type),
            Key = ObjectIdentityCalculator.CalculateHashHexKey(data),
            Id = ObjectIdentityCalculator.CalculateHashHex(data)
        };
    }

    public IObjectIdentifier GetObjectIdentifier(IObjectData data)
    {
        return new ObjectIdentifier()
        {
            Type = data.Type,
            StorageIdentifier = ObjectStorageManager.GetObjectStorageHolder(data.Type),
            Id = ObjectIdentityCalculator.CalculateHashHex(data)
        };
    }
}