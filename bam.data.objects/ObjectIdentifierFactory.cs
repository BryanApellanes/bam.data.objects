using Bam.Data.Objects;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

public class ObjectIdentifierFactory : IObjectIdentifierFactory
{
    public ObjectIdentifierFactory(IObjectStorageManager objectStorageManager, IObjectHashCalculator objectHashCalculator)
    {
        this.ObjectStorageManager = objectStorageManager;
        this.ObjectHashCalculator = objectHashCalculator;
    }

    private IObjectStorageManager ObjectStorageManager { get; init; }
    private IObjectHashCalculator ObjectHashCalculator { get; init; }

    public IObjectKey GetObjectKey(IObjectData data)
    {
        return new ObjectKey()
        {
            Type = data.Type,
            StorageIdentifier = ObjectStorageManager.GetTypeStorageContainer(data.Type),
            Key = ObjectHashCalculator.CalculateKeyHash(data),
            Hash = ObjectHashCalculator.CalculateHash(data)
        };
    }

    public IObjectIdentifier GetObjectIdentifier(IObjectData data)
    {
        return new ObjectIdentifier()
        {
            Type = data.Type,
            StorageIdentifier = ObjectStorageManager.GetTypeStorageContainer(data.Type),
            Hash = ObjectHashCalculator.CalculateHash(data)
        };
    }
}