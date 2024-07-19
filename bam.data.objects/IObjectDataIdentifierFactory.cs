namespace Bam.Data.Objects;

public interface IObjectDataIdentifierFactory
{
    IObjectDataKey GetObjectKey(IObjectData data);
    IObjectDataIdentifier GetObjectIdentifier(IObjectData data);
    
}