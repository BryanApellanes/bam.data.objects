namespace Bam.Data.Objects;

public class ObjectDataFactory : IObjectDataFactory
{
    public ObjectDataFactory(IObjectDataIdentifierFactory objectDataIdentifierFactory, IObjectEncoderDecoder encoderDecoder)
    {
        this.ObjectDataIdentifierFactory = objectDataIdentifierFactory;
        this.ObjectEncoderDecoder = encoderDecoder;
    }
    
    public IObjectDataIdentifierFactory ObjectDataIdentifierFactory { get; init; }
    public  IObjectEncoderDecoder ObjectEncoderDecoder { get; init; }

    public IObjectData Wrap(object data)
    {
        if (data is ObjectData objectData)
        {
            objectData.ObjectDataIdentifierFactory ??= this.ObjectDataIdentifierFactory;
            return objectData;
        }

        if (data is IObjectData iObjectData)
        {
            iObjectData.ObjectDataIdentifierFactory ??= this.ObjectDataIdentifierFactory;
            return iObjectData;
        }

        return new ObjectData(data, ObjectEncoderDecoder){ObjectDataIdentifierFactory = this.ObjectDataIdentifierFactory};
    }

    public IObjectDataKey GetObjectKey(IObjectData data)
    {
        return ObjectDataIdentifierFactory.GetObjectKey(data);
    }

    public IObjectDataIdentifier GetObjectIdentifier(IObjectData data)
    {
        return ObjectDataIdentifierFactory.GetObjectIdentifier(data);
    }
}