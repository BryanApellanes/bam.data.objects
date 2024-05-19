using Bam.Data.Objects;
using Bam.Storage;

namespace Bam.Data.Dynamic.Objects;

public class ObjectIdentifierFactory : IObjectIdentifierFactory
{
    public ObjectIdentifierFactory(IObjectStorageManager objectStorageManager, IObjectCalculator objectCalculator)
    {
        this.ObjectStorageManager = objectStorageManager;
        this.ObjectCalculator = objectCalculator;
    }

    private IObjectStorageManager ObjectStorageManager { get; init; }
    private IObjectCalculator ObjectCalculator { get; init; }

    public IObjectKey GetObjectKey(IObjectData data)
    {
        return new ObjectKey()
        {
            Type = data.Type,
            StorageIdentifier = ObjectStorageManager.GetTypeStorageHolder(data.Type),
            Key = ObjectCalculator.CalculateHashHexKey(data),
            Id = ObjectCalculator.CalculateHashHex(data)
        };
    }

    public IObjectIdentifier GetObjectIdentifier(IObjectData data)
    {
        return new ObjectIdentifier()
        {
            Type = data.Type,
            StorageIdentifier = ObjectStorageManager.GetTypeStorageHolder(data.Type),
            Id = ObjectCalculator.CalculateHashHex(data)
        };
    }
}