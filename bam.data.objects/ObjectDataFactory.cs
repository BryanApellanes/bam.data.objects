namespace Bam.Data.Objects;

public class ObjectDataFactory : IObjectDataFactory
{
    public ObjectDataFactory(IObjectIdentifierFactory objectIdentifierFactory) : this(objectIdentifierFactory, JsonObjectEncoder.Default)
    {
    }

    public ObjectDataFactory(IObjectIdentifierFactory objectIdentifierFactory, ObjectEncoder encoder)
    {
        this.ObjectIdentifierFactory = objectIdentifierFactory;
        this.ObjectEncoder = encoder;
    }
    
    public IObjectIdentifierFactory ObjectIdentifierFactory { get; init; }
    public  ObjectEncoder ObjectEncoder { get; init; }

    public IObjectData Wrap(object data)
    {
        if (data is ObjectData objectData)
        {
            return objectData;
        }

        if (data is IObjectData iObjectData)
        {
            return iObjectData;
        }
        
        return new ObjectData(data, ObjectEncoder);
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