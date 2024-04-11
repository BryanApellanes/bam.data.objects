namespace Bam.Data.Objects;

public interface IObjectIdentifierFactory
{
    IObjectKey GetObjectKey(IObjectData data);
    IObjectIdentifier GetObjectIdentifier(IObjectData data);
    
}