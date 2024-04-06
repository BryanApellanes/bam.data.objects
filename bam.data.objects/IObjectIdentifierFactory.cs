namespace Bam.Data.Objects;

public interface IObjectIdentifierFactory
{
    IObjectKey GetObjectKeyFor(IObjectData data);
    IObjectIdentifier GetObjectIdentifierFor(IObjectData data);
    
}