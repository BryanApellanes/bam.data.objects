using Bam.Data.Objects;

namespace Bam.Data.Dynamic.Objects;

/// <summary>
/// Default implementation of <see cref="IObjectDataLocatorFactory"/> that uses an <see cref="IObjectDataIdentityCalculator"/> to compute keys and identifiers.
/// </summary>
public class ObjectDataLocatorFactory : IObjectDataLocatorFactory
{
    /// <summary>
    /// Initializes a new instance of the <see cref="ObjectDataLocatorFactory"/> class.
    /// </summary>
    /// <param name="objectDataIdentityCalculator">The identity calculator used to compute hash keys and identifiers.</param>
    public ObjectDataLocatorFactory(IObjectDataIdentityCalculator objectDataIdentityCalculator)
    {
        this.ObjectDataIdentityCalculator = objectDataIdentityCalculator;
    }
    private IObjectDataIdentityCalculator ObjectDataIdentityCalculator { get; init; }

    /// <inheritdoc />
    public IObjectDataLocator GetObjectDataLocator(IObjectDataStorageManager storageManager, IObjectData data)
    {
        return new ObjectDataLocator()
        {
            StorageIdentifier = storageManager.GetObjectStorageHolder(data.TypeDescriptor),
            ObjectDataKey = GetObjectKey(data),
            ObjectDataIdentifier = GetObjectIdentifier(data)
        };
    }

    /// <inheritdoc />
    public IObjectDataKey GetObjectKey(IObjectData data)
    {
        return new ObjectDataKey()
        {
            TypeDescriptor = data.TypeDescriptor,
            Key = ObjectDataIdentityCalculator.CalculateHashHexKey(data),
        };
    }

    /// <inheritdoc />
    public IObjectDataIdentifier GetObjectIdentifier(IObjectData data)
    {
        return new ObjectDataIdentifier()
        {
            TypeDescriptor = data.TypeDescriptor,
            Id = ObjectDataIdentityCalculator.CalculateHashHex(data)
        };
    }
}