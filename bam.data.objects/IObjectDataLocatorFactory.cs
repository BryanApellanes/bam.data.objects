namespace Bam.Data.Objects;

public interface IObjectDataLocatorFactory
{
    IObjectDataLocator GetObjectDataLocator(IObjectDataStorageManager storageManager, IObjectData data);
    IObjectDataKey GetObjectKey(IObjectData data);
    IObjectDataIdentifier GetObjectIdentifier(IObjectData data);
    
}