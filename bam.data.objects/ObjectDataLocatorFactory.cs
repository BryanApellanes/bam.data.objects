using Bam.Data.Objects;

namespace Bam.Data.Dynamic.Objects;

public class ObjectDataLocatorFactory : IObjectDataLocatorFactory
{
    public ObjectDataLocatorFactory(IObjectDataIdentityCalculator objectDataIdentityCalculator)
    {
        this.ObjectDataIdentityCalculator = objectDataIdentityCalculator;
    }
    private IObjectDataIdentityCalculator ObjectDataIdentityCalculator { get; init; }

    public IObjectDataLocator GetObjectDataLocator(IObjectDataStorageManager storageManager, IObjectData data)
    {
        return new ObjectDataLocator()
        {
            StorageIdentifier = storageManager.GetObjectStorageHolder(data.TypeDescriptor),
            ObjectDataKey = GetObjectKey(data),
            ObjectDataIdentifier = GetObjectIdentifier(data)
        };
    }

    public IObjectDataKey GetObjectKey(IObjectData data)
    {
        return new ObjectDataKey()
        {
            TypeDescriptor = data.TypeDescriptor,
            Key = ObjectDataIdentityCalculator.CalculateHashHexKey(data),
        };
    }

    public IObjectDataIdentifier GetObjectIdentifier(IObjectData data)
    {
        return new ObjectDataIdentifier()
        {
            TypeDescriptor = data.TypeDescriptor,
            Id = ObjectDataIdentityCalculator.CalculateHashHex(data)
        };
    }
}