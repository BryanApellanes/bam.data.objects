namespace Bam.Data.Objects;

public class ObjectDataFactory : IObjectDataFactory
{
    public ObjectDataFactory(IObjectIdentifierFactory objectIdentifierFactory, IObjectEncoderDecoder encoderDecoder)
    {
        this.ObjectIdentifierFactory = objectIdentifierFactory;
        this.ObjectEncoderDecoder = encoderDecoder;
    }
    
    public IObjectIdentifierFactory ObjectIdentifierFactory { get; init; }
    public  IObjectEncoderDecoder ObjectEncoderDecoder { get; init; }

    public IObjectData Wrap(object data)
    {
        if (data is ObjectData objectData)
        {
            objectData.ObjectIdentifierFactory ??= this.ObjectIdentifierFactory;
            return objectData;
        }

        if (data is IObjectData iObjectData)
        {
            iObjectData.ObjectIdentifierFactory ??= this.ObjectIdentifierFactory;
            return iObjectData;
        }

        return new ObjectData(data, ObjectEncoderDecoder){ObjectIdentifierFactory = this.ObjectIdentifierFactory};
    }

    public IObjectKey GetObjectKey(IObjectData data)
    {
        return ObjectIdentifierFactory.GetObjectKey(data);
    }

    public IObjectIdentifier GetObjectIdentifier(IObjectData data)
    {
        return ObjectIdentifierFactory.GetObjectIdentifier(data);
    }
}