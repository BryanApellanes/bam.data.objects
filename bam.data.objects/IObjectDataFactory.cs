namespace Bam.Data.Objects;

public interface IObjectDataFactory
{
    IObjectDataIdentifierFactory ObjectDataIdentifierFactory { get; }
    IObjectEncoderDecoder ObjectEncoderDecoder { get; }
    
    IObjectData Wrap(object data);
    IObjectDataKey GetObjectKey(IObjectData data);
    IObjectDataIdentifier GetObjectIdentifier(IObjectData data);
}